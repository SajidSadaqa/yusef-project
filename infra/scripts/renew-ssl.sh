#!/bin/bash

##############################################################################
# SSL Certificate Renewal Script
##############################################################################
# This script manually renews SSL certificates
# Usually not needed as auto-renewal is set up, but useful for testing
##############################################################################

set -e

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== SSL Certificate Renewal ===${NC}"

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo -e "${RED}Please run as root (use sudo)${NC}"
    exit 1
fi

# Renew certificates
echo -e "${GREEN}Checking and renewing certificates...${NC}"
certbot renew --quiet

# Reload nginx
echo -e "${GREEN}Reloading nginx...${NC}"
docker compose restart nginx

echo -e "${GREEN}=== Renewal Complete! ===${NC}"
echo -e "${YELLOW}If certificates were renewed, nginx has been reloaded${NC}"

# Show certificate expiration
echo -e "${GREEN}Certificate expiration dates:${NC}"
certbot certificates
