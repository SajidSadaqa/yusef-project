#!/bin/bash

##############################################################################
# Production Deployment Script
##############################################################################
# This script deploys or updates the Vertex Transport application
##############################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}╔════════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║     Vertex Transport - Production Deployment Script           ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Check if running from project root
if [ ! -f "docker-compose.prod.yml" ]; then
    echo -e "${RED}Error: Please run this script from the project root directory${NC}"
    exit 1
fi

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo -e "${RED}Error: .env file not found!${NC}"
    echo -e "${YELLOW}Please copy .env.example to .env and configure it first${NC}"
    echo -e "${YELLOW}Run: cp .env.example .env${NC}"
    exit 1
fi

# Check if DOMAIN_NAME is set
if ! grep -q "DOMAIN_NAME=" .env || grep -q "DOMAIN_NAME=yourdomain.com" .env; then
    echo -e "${RED}Error: DOMAIN_NAME not configured in .env${NC}"
    echo -e "${YELLOW}Please set your actual domain name in .env file${NC}"
    exit 1
fi

# Function to check if Docker is installed
check_docker() {
    if ! command -v docker &> /dev/null; then
        echo -e "${RED}Docker is not installed!${NC}"
        echo -e "${YELLOW}Install Docker first: curl -fsSL https://get.docker.com | sh${NC}"
        exit 1
    fi

    if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
        echo -e "${RED}Docker Compose is not installed!${NC}"
        exit 1
    fi

    echo -e "${GREEN}✓ Docker is installed${NC}"
}

# Function to create backup
create_backup() {
    echo -e "${BLUE}Creating database backup...${NC}"

    BACKUP_DIR="./backups"
    mkdir -p "$BACKUP_DIR"

    DATE=$(date +%Y%m%d_%H%M%S)
    BACKUP_FILE="$BACKUP_DIR/db_backup_$DATE.sql.gz"

    # Check if postgres container is running
    if docker ps | grep -q postgres; then
        docker exec postgres pg_dump -U shipmentuser shipment_tracking_prod | gzip > "$BACKUP_FILE" 2>/dev/null || true
        if [ -f "$BACKUP_FILE" ]; then
            echo -e "${GREEN}✓ Backup created: $BACKUP_FILE${NC}"
        else
            echo -e "${YELLOW}⚠ Could not create backup (database may be new)${NC}"
        fi
    else
        echo -e "${YELLOW}⚠ Postgres not running, skipping backup${NC}"
    fi
}

# Function to pull latest code
pull_code() {
    echo -e "${BLUE}Pulling latest code...${NC}"

    if [ -d ".git" ]; then
        git pull
        echo -e "${GREEN}✓ Code updated${NC}"
    else
        echo -e "${YELLOW}⚠ Not a git repository, skipping pull${NC}"
    fi
}

# Function to build and deploy
deploy() {
    echo -e "${BLUE}Building and deploying containers...${NC}"

    # Build images
    docker-compose -f docker-compose.prod.yml build --no-cache

    # Start services
    docker-compose -f docker-compose.prod.yml up -d

    echo -e "${GREEN}✓ Containers deployed${NC}"
}

# Function to check health
check_health() {
    echo -e "${BLUE}Checking service health...${NC}"
    sleep 5

    # Wait for services to be healthy (max 60 seconds)
    echo -e "${YELLOW}Waiting for services to start...${NC}"
    COUNTER=0
    while [ $COUNTER -lt 12 ]; do
        if docker-compose -f docker-compose.prod.yml ps | grep -q "unhealthy"; then
            echo -e "${YELLOW}Services still starting... ($((COUNTER * 5))s)${NC}"
            sleep 5
            COUNTER=$((COUNTER + 1))
        else
            break
        fi
    done

    # Check container status
    echo ""
    docker-compose -f docker-compose.prod.yml ps
    echo ""

    # Test endpoints
    echo -e "${BLUE}Testing endpoints...${NC}"

    if curl -f http://localhost/healthz &> /dev/null; then
        echo -e "${GREEN}✓ Nginx health check passed${NC}"
    else
        echo -e "${RED}✗ Nginx health check failed${NC}"
    fi

    if curl -f http://localhost/api/healthz/ready &> /dev/null; then
        echo -e "${GREEN}✓ Backend health check passed${NC}"
    else
        echo -e "${RED}✗ Backend health check failed${NC}"
    fi
}

# Function to show logs
show_logs() {
    echo -e "${BLUE}Recent logs:${NC}"
    echo -e "${YELLOW}─────────────────────────────────────────────────────────────${NC}"
    docker-compose -f docker-compose.prod.yml logs --tail=20
    echo -e "${YELLOW}─────────────────────────────────────────────────────────────${NC}"
}

# Main deployment flow
main() {
    echo -e "${BLUE}Starting deployment process...${NC}"
    echo ""

    # Pre-deployment checks
    check_docker

    # Ask for confirmation
    echo ""
    read -p "$(echo -e ${YELLOW}Continue with deployment? [y/N]: ${NC})" -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}Deployment cancelled${NC}"
        exit 1
    fi

    # Deployment steps
    create_backup
    echo ""

    pull_code
    echo ""

    deploy
    echo ""

    check_health
    echo ""

    show_logs
    echo ""

    # Success message
    echo -e "${GREEN}╔════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${GREEN}║              Deployment completed successfully!                ║${NC}"
    echo -e "${GREEN}╚════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo -e "1. Verify site is accessible at your domain"
    echo -e "2. Test critical user flows"
    echo -e "3. Monitor logs: ${YELLOW}docker-compose -f docker-compose.prod.yml logs -f${NC}"
    echo -e "4. Check metrics and monitoring dashboards"
    echo ""
    echo -e "${YELLOW}To view logs:${NC} docker-compose -f docker-compose.prod.yml logs -f"
    echo -e "${YELLOW}To restart:${NC}  docker-compose -f docker-compose.prod.yml restart [service]"
    echo -e "${YELLOW}To stop:${NC}     docker-compose -f docker-compose.prod.yml down"
    echo ""
}

# Run main function
main
