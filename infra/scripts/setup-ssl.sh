#!/bin/bash

##############################################################################
# SSL Certificate Setup Script (Let's Encrypt with Certbot)
##############################################################################
# This script sets up SSL certificates for your domain using Let's Encrypt
# Run this on your production server after deploying the application
##############################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== SSL Certificate Setup with Let's Encrypt ===${NC}"

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo -e "${RED}Please run as root (use sudo)${NC}"
    exit 1
fi

# Prompt for domain name
read -p "Enter your domain name (e.g., example.com): " DOMAIN_NAME

if [ -z "$DOMAIN_NAME" ]; then
    echo -e "${RED}Domain name is required!${NC}"
    exit 1
fi

read -p "Enter your email for Let's Encrypt notifications: " EMAIL

if [ -z "$EMAIL" ]; then
    echo -e "${RED}Email is required!${NC}"
    exit 1
fi

echo -e "${YELLOW}Domain: $DOMAIN_NAME${NC}"
echo -e "${YELLOW}Email: $EMAIL${NC}"
read -p "Continue? (y/n): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    exit 1
fi

# Install certbot if not already installed
echo -e "${GREEN}Installing certbot...${NC}"
if ! command -v certbot &> /dev/null; then
    apt-get update
    apt-get install -y certbot
fi

# Stop nginx container temporarily
echo -e "${GREEN}Stopping nginx container...${NC}"
docker compose stop nginx || true

# Generate certificate using standalone mode
echo -e "${GREEN}Generating SSL certificate...${NC}"
certbot certonly --standalone \
    --non-interactive \
    --agree-tos \
    --email "$EMAIL" \
    -d "$DOMAIN_NAME" \
    -d "www.$DOMAIN_NAME" \
    --preferred-challenges http

if [ $? -eq 0 ]; then
    echo -e "${GREEN}SSL certificate generated successfully!${NC}"
else
    echo -e "${RED}Failed to generate SSL certificate${NC}"
    exit 1
fi

# Generate Diffie-Hellman parameters for enhanced security
echo -e "${GREEN}Generating Diffie-Hellman parameters (this may take a while)...${NC}"
if [ ! -f /etc/nginx/dhparam.pem ]; then
    mkdir -p /etc/nginx
    openssl dhparam -out /etc/nginx/dhparam.pem 4096
    echo -e "${GREEN}DH parameters generated!${NC}"
else
    echo -e "${YELLOW}DH parameters already exist, skipping...${NC}"
fi

# Set up auto-renewal
echo -e "${GREEN}Setting up auto-renewal...${NC}"
cat > /etc/cron.d/certbot-renew << EOF
# Renew certificates twice daily and reload nginx if renewed
0 */12 * * * root certbot renew --quiet --deploy-hook "docker compose -f /root/shipment-tracking/docker-compose.yml restart nginx"
EOF

echo -e "${GREEN}Auto-renewal configured!${NC}"

# Update environment file with domain name
echo -e "${GREEN}Updating environment configuration...${NC}"
if ! grep -q "DOMAIN_NAME=" .env 2>/dev/null; then
    echo "DOMAIN_NAME=$DOMAIN_NAME" >> .env
else
    sed -i "s/DOMAIN_NAME=.*/DOMAIN_NAME=$DOMAIN_NAME/" .env
fi

# Restart nginx with SSL
echo -e "${GREEN}Starting nginx with SSL...${NC}"
docker compose up -d nginx

echo -e "${GREEN}=== SSL Setup Complete! ===${NC}"
echo -e "${GREEN}Your site should now be accessible at https://$DOMAIN_NAME${NC}"
echo -e "${YELLOW}Certificates will auto-renew twice daily${NC}"
echo -e "${YELLOW}Certificate location: /etc/letsencrypt/live/$DOMAIN_NAME/${NC}"
