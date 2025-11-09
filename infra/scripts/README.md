# Deployment Scripts

This directory contains automated scripts to simplify deployment and maintenance tasks.

## Available Scripts

### 1. SSL Setup Script (`setup-ssl.sh`)

Automates the complete SSL certificate setup process using Let's Encrypt.

**Usage:**
```bash
chmod +x infra/scripts/setup-ssl.sh
sudo ./infra/scripts/setup-ssl.sh
```

**What it does:**
- Installs Certbot if not already installed
- Generates SSL certificates for your domain (both www and non-www)
- Creates Diffie-Hellman parameters for enhanced security (4096-bit)
- Configures automatic certificate renewal (twice daily)
- Updates your .env file with the domain name
- Restarts nginx with HTTPS enabled

**Requirements:**
- Run as root (sudo)
- Domain must be pointed to your server IP
- nginx container must be stoppable
- Port 80 must be accessible from the internet

**Interactive prompts:**
- Domain name (e.g., example.com)
- Email address for Let's Encrypt notifications

---

### 2. SSL Renewal Script (`renew-ssl.sh`)

Manually renews SSL certificates (usually not needed due to auto-renewal).

**Usage:**
```bash
chmod +x infra/scripts/renew-ssl.sh
sudo ./infra/scripts/renew-ssl.sh
```

**What it does:**
- Checks all certificates for renewal eligibility
- Renews certificates that are close to expiration
- Reloads nginx to apply new certificates
- Shows certificate expiration dates

**When to use:**
- Testing the renewal process
- Forcing renewal before expiration
- Troubleshooting certificate issues

---

### 3. Deployment Script (`deploy.sh`)

Automates the deployment or update process for the application.

**Usage:**
```bash
chmod +x infra/scripts/deploy.sh
./infra/scripts/deploy.sh
```

**What it does:**
1. Checks Docker is installed
2. Creates a database backup before deployment
3. Pulls latest code from git (if in a git repository)
4. Builds Docker images with no cache
5. Starts/restarts all services
6. Performs health checks on all services
7. Shows recent logs

**Use cases:**
- Initial deployment to production
- Deploying updates to the application
- Quick redeployment after configuration changes

**Safety features:**
- Requires confirmation before proceeding
- Creates database backup automatically
- Shows service health status after deployment

---

## Script Permissions

Make all scripts executable:
```bash
chmod +x infra/scripts/*.sh
```

## Prerequisites

### For SSL Scripts:
- Ubuntu/Debian-based system
- Root access (sudo)
- Port 80 accessible from internet
- Valid domain pointing to server

### For Deployment Script:
- Docker and Docker Compose installed
- Access to the application directory
- .env file configured
- (Optional) Git repository access

## Automated Processes

Once you run `setup-ssl.sh`, the following happens automatically:

### Certificate Auto-Renewal
- **Frequency:** Twice daily (0:00 and 12:00)
- **Trigger:** Certificates with <30 days remaining
- **Action:** Renew certificate and restart nginx
- **Managed by:** Cron job in `/etc/cron.d/certbot-renew`

**Check auto-renewal status:**
```bash
sudo crontab -l | grep certbot
sudo certbot renew --dry-run
```

## Troubleshooting

### SSL Setup Fails

**Certificate generation failed:**
```bash
# Check DNS is pointing to your server
nslookup yourdomain.com

# Ensure port 80 is accessible
sudo ufw status
curl -I http://yourdomain.com
```

**Domain validation timeout:**
- Wait for DNS propagation (can take up to 48 hours)
- Ensure firewall allows HTTP traffic
- Verify domain A record is correct

### Deployment Fails

**Docker not found:**
```bash
# Install Docker
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
```

**Permission denied:**
```bash
# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker
```

**Container health check fails:**
```bash
# Check logs
docker-compose -f docker-compose.prod.yml logs [service]

# Check environment variables
cat .env
```

## Manual Operations

### Check SSL Certificate Status
```bash
sudo certbot certificates
```

### Force SSL Renewal
```bash
sudo certbot renew --force-renewal
```

### View Certificate Details
```bash
sudo openssl x509 -in /etc/letsencrypt/live/yourdomain.com/fullchain.pem -text -noout
```

### Test SSL Configuration
```bash
# Test nginx configuration
docker exec nginx nginx -t

# Check SSL Labs rating
# Visit: https://www.ssllabs.com/ssltest/analyze.html?d=yourdomain.com
```

### Manual Deployment
```bash
# Pull latest code
git pull origin main

# Backup database
docker exec postgres pg_dump -U shipmentuser shipment_tracking_prod | gzip > backup.sql.gz

# Rebuild and restart
docker-compose -f docker-compose.prod.yml up -d --build

# Check status
docker-compose -f docker-compose.prod.yml ps
```

## Security Notes

1. **Never commit these scripts with hardcoded credentials**
2. **SSL scripts must run as root (they modify system files)**
3. **Deployment script should run as the deploy user, not root**
4. **Backup files contain sensitive data - store securely**
5. **Review logs regularly for security issues**

## Support

- SSL issues: https://community.letsencrypt.org/
- Docker issues: https://docs.docker.com/
- Application issues: See [DEPLOYMENT_HETZNER.md](../../DEPLOYMENT_HETZNER.md)

## Quick Reference

```bash
# Initial SSL setup
sudo ./infra/scripts/setup-ssl.sh

# Deploy/update application
./infra/scripts/deploy.sh

# Renew SSL manually (if needed)
sudo ./infra/scripts/renew-ssl.sh

# Check SSL status
sudo certbot certificates

# View application logs
docker-compose -f docker-compose.prod.yml logs -f

# Restart a service
docker-compose -f docker-compose.prod.yml restart [service]

# Check service health
docker-compose -f docker-compose.prod.yml ps
```

---

**Last Updated:** November 2025
**Compatibility:** Ubuntu 22.04 LTS, Docker 24+, Docker Compose 2.0+
