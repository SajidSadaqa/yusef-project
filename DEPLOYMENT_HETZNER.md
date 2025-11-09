# Vertex Transport - Hetzner Cloud Deployment Guide

Complete guide for deploying the Vertex Transport shipment tracking application on Hetzner Cloud.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Server Selection & Setup](#server-selection--setup)
3. [Initial Server Configuration](#initial-server-configuration)
4. [Domain & DNS Configuration](#domain--dns-configuration)
5. [Application Deployment](#application-deployment)
6. [SSL Certificate Setup](#ssl-certificate-setup)
7. [Production Configuration](#production-configuration)
8. [Backup & Monitoring](#backup--monitoring)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Accounts
- [ ] Hetzner Cloud account (https://www.hetzner.com/)
- [ ] Domain name (from any registrar: Namecheap, GoDaddy, Cloudflare, etc.)
- [ ] Git repository access
- [ ] Email service account (Resend.com recommended)

### Local Requirements
- SSH client (Terminal on Mac/Linux, PuTTY on Windows)
- Git installed locally
- Text editor (VS Code, Sublime, etc.)

---

## Server Selection & Setup

### Recommended Server Configuration

For production deployment, we recommend:

**VPS Plan: CPX31** (€13.46/month)
- 4 vCPUs (AMD or Intel)
- 8 GB RAM
- 160 GB NVMe SSD
- 20 TB Traffic
- IPv4 & IPv6

**Alternative for smaller deployments: CPX21** (€7.38/month)
- 3 vCPUs
- 4 GB RAM
- 80 GB NVMe SSD
- Good for testing or low-traffic sites

### Step 1: Create Hetzner Cloud Server

1. **Login to Hetzner Cloud Console**
   - Go to https://console.hetzner.cloud/
   - Click "New Project" or select existing project

2. **Create Server**
   ```
   Click "+ NEW SERVER"

   Location: Choose closest to your users
   - Nuremberg, Germany (EU)
   - Helsinki, Finland (EU)
   - Ashburn, VA (US)

   Image: Ubuntu 22.04 LTS

   Type: Shared vCPU
   Plan: CPX31 (or CPX21 for testing)

   Networking:
   ☑ Public IPv4
   ☑ Public IPv6

   SSH Keys: Add your SSH public key
   (Generate one if needed: ssh-keygen -t ed25519 -C "your_email@example.com")

   Volumes: None (we'll add later for backups if needed)

   Firewalls: Create new firewall:
   - Name: "vertex-transport-firewall"
   - Inbound Rules:
     * SSH (Port 22) - Your IP only (for security)
     * HTTP (Port 80) - All IPv4/IPv6
     * HTTPS (Port 443) - All IPv4/IPv6

   Backups: Enable (recommended, adds 20% to cost)

   Server Name: vertex-transport-prod
   ```

3. **Note the Server IP**
   - After creation, note the IPv4 address (e.g., 123.456.789.012)
   - You'll need this for DNS and SSH access

---

## Initial Server Configuration

### Step 2: Connect to Server

```bash
# Replace with your server IP
ssh root@YOUR_SERVER_IP

# Accept the SSH fingerprint
```

### Step 3: Update System

```bash
# Update package list and upgrade system
apt update && apt upgrade -y

# Install essential packages
apt install -y curl wget git ufw fail2ban

# Reboot if kernel was updated
reboot
```

Wait 1-2 minutes, then reconnect:
```bash
ssh root@YOUR_SERVER_IP
```

### Step 4: Configure Firewall

```bash
# Configure UFW (Uncomplicated Firewall)
ufw default deny incoming
ufw default allow outgoing
ufw allow ssh
ufw allow http
ufw allow https

# Enable firewall
ufw enable

# Check status
ufw status
```

### Step 5: Create Non-Root User (Security Best Practice)

```bash
# Create deploy user
adduser deploy

# Add to sudo group
usermod -aG sudo deploy

# Copy SSH keys to new user
rsync --archive --chown=deploy:deploy ~/.ssh /home/deploy

# Test new user (in a new terminal)
ssh deploy@YOUR_SERVER_IP
sudo apt update  # Test sudo access
```

From now on, use the `deploy` user instead of root.

### Step 6: Install Docker & Docker Compose

```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add deploy user to docker group
sudo usermod -aG docker deploy

# Apply group changes
newgrp docker

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installations
docker --version
docker-compose --version
```

---

## Domain & DNS Configuration

### Step 7: Configure DNS Records

In your domain registrar (Namecheap, GoDaddy, Cloudflare, etc.):

```
Type    Name    Value                       TTL
----    ----    -----                       ---
A       @       YOUR_SERVER_IP              3600
A       www     YOUR_SERVER_IP              3600
AAAA    @       YOUR_SERVER_IPv6 (optional) 3600
AAAA    www     YOUR_SERVER_IPv6 (optional) 3600
```

**Example:**
```
A       @       123.456.789.012
A       www     123.456.789.012
```

### Step 8: Verify DNS Propagation

Wait 5-30 minutes, then verify:

```bash
# Check from your local machine
nslookup yourdomain.com
ping yourdomain.com

# Should show your Hetzner server IP
```

---

## Application Deployment

### Step 9: Clone Repository

```bash
# Create app directory
mkdir -p /home/deploy/apps
cd /home/deploy/apps

# Clone your repository
git clone https://github.com/YOUR_USERNAME/vertex-transport.git
cd vertex-transport

# Or use HTTPS with token
git clone https://YOUR_TOKEN@github.com/YOUR_USERNAME/vertex-transport.git
```

### Step 10: Configure Environment Variables

```bash
# Copy example env file
cp .env.example .env

# Edit with your values
nano .env
```

**Required Changes in .env:**

```bash
# Database
POSTGRES_PASSWORD=YOUR_STRONG_PASSWORD_HERE

# JWT (minimum 32 chars)
JWT_SECRET=YOUR_RANDOM_STRING_HERE

# Email
RESEND_API_KEY=re_your_api_key
RESEND_FROM_EMAIL=noreply@yourdomain.com

# CORS
CORS_ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com

# URLs
NEXT_PUBLIC_API_URL=https://yourdomain.com/api
NEXT_PUBLIC_SITE_URL=https://yourdomain.com

# Domain
DOMAIN=yourdomain.com
LETSENCRYPT_EMAIL=admin@yourdomain.com
```

**Generate Strong Secrets:**
```bash
# Generate JWT secret
openssl rand -base64 48

# Generate database password
openssl rand -base64 32
```

### Step 11: Update Nginx Configuration

```bash
# Edit nginx config
nano infra/nginx/default.conf
```

Update the HTTPS server block:
```nginx
# Uncomment the HTTPS server block (lines 52-130)
# Replace yourdomain.com with your actual domain
# Replace www.yourdomain.com if you're using www subdomain
```

### Step 12: Initial Deployment (HTTP Only)

```bash
# Build and start containers
docker-compose -f docker-compose.prod.yml up -d --build

# Check container status
docker-compose -f docker-compose.prod.yml ps

# View logs
docker-compose -f docker-compose.prod.yml logs -f
```

Visit `http://yourdomain.com` to verify the app is running.

---

## SSL Certificate Setup

### Step 13: Automated SSL Setup (Recommended)

We provide an automated script for SSL certificate setup:

```bash
# Navigate to app directory
cd /home/deploy/apps/vertex-transport

# Make SSL setup script executable
chmod +x infra/scripts/setup-ssl.sh

# Run the SSL setup script as root
sudo ./infra/scripts/setup-ssl.sh
```

The script will:
- Install Certbot if not present
- Generate SSL certificates using Let's Encrypt
- Create Diffie-Hellman parameters for enhanced security
- Configure automatic certificate renewal
- Update your .env file with domain name
- Restart nginx with HTTPS enabled

### Step 14: Manual SSL Setup (Alternative)

If you prefer manual setup:

**Step 14a: Install Certbot**

```bash
# Install Certbot
sudo apt install -y certbot
```

**Step 14b: Obtain SSL Certificate**

```bash
# Stop nginx temporarily
docker-compose -f docker-compose.prod.yml stop nginx

# Obtain certificate using standalone mode
sudo certbot certonly --standalone \
  --non-interactive \
  --agree-tos \
  --email admin@yourdomain.com \
  -d yourdomain.com \
  -d www.yourdomain.com \
  --preferred-challenges http

# Note the certificate paths shown
# Usually: /etc/letsencrypt/live/yourdomain.com/
```

**Step 14c: Generate DH Parameters**

```bash
# Generate Diffie-Hellman parameters (takes 5-10 minutes)
sudo mkdir -p /etc/nginx
sudo openssl dhparam -out /etc/nginx/dhparam.pem 4096
```

**Step 14d: Update Environment Variables**

```bash
# Add domain name to .env file
echo "DOMAIN_NAME=yourdomain.com" >> .env
```

**Step 14e: Restart Services**

```bash
# Restart nginx with HTTPS enabled
docker-compose -f docker-compose.prod.yml up -d nginx

# Check nginx is running with HTTPS
docker-compose -f docker-compose.prod.yml ps nginx
```

### Step 15: Test HTTPS

```bash
# Test HTTPS is working
curl -I https://yourdomain.com

# Test HTTP redirect
curl -I http://yourdomain.com
# Should return 301 redirect to https://
```

**Browser Test:**
- Visit `https://yourdomain.com` - should show secure padlock icon
- Visit `http://yourdomain.com` - should redirect to HTTPS

**SSL Quality Test:**
- Test SSL configuration: https://www.ssllabs.com/ssltest/analyze.html?d=yourdomain.com
- Target: A or A+ rating

### Step 16: Certificate Auto-Renewal

The setup script automatically configures renewal. To manually verify:

```bash
# Check auto-renewal cron job
sudo crontab -l | grep certbot

# Test renewal (dry run)
sudo certbot renew --dry-run

# Manual renewal script is available
chmod +x infra/scripts/renew-ssl.sh
sudo ./infra/scripts/renew-ssl.sh

# Check certificate expiration
sudo certbot certificates
```

**Certificate Lifecycle:**
- Certificates are valid for 90 days
- Auto-renewal runs twice daily
- Renewal occurs when certificate has <30 days remaining
- Nginx automatically restarts after successful renewal

---

## Production Configuration

### Step 18: Update Backend for Production

```bash
# Verify production settings in appsettings.Production.json exist
cat shipment-track-backend/src/ShipmentTracking.WebApi/appsettings.Production.json

# Rebuild backend
docker-compose -f docker-compose.prod.yml up -d --build backend
```

### Step 19: Verify All Services

```bash
# Check all services are running
docker-compose -f docker-compose.prod.yml ps

# Should show:
# nginx       - Up (healthy)
# frontend    - Up (healthy)
# backend     - Up (healthy)
# postgres    - Up (healthy)

# Test endpoints
curl https://yourdomain.com/healthz        # nginx health
curl https://yourdomain.com/api/healthz/ready  # backend health
```

### Step 20: Initialize Admin User

```bash
# Backend automatically seeds admin user on first run
# Check logs to confirm
docker-compose -f docker-compose.prod.yml logs backend | grep "Admin user"

# Login credentials are from your .env file:
# Email: ADMIN_EMAIL
# Password: ADMIN_PASSWORD
```

---

## Backup & Monitoring

### Step 21: Setup Database Backups

```bash
# Create backup script
mkdir -p /home/deploy/backups
nano /home/deploy/backups/backup-db.sh
```

```bash
#!/bin/bash
BACKUP_DIR="/home/deploy/backups"
DATE=$(date +%Y%m%d_%H%M%S)
DB_NAME="shipment_tracking_prod"

docker exec postgres pg_dump -U shipmentuser $DB_NAME | gzip > $BACKUP_DIR/db_backup_$DATE.sql.gz

# Keep only last 30 days of backups
find $BACKUP_DIR -name "db_backup_*.sql.gz" -mtime +30 -delete

echo "Backup completed: db_backup_$DATE.sql.gz"
```

```bash
# Make executable
chmod +x /home/deploy/backups/backup-db.sh

# Add to crontab (daily at 2 AM)
crontab -e

# Add line:
0 2 * * * /home/deploy/backups/backup-db.sh >> /home/deploy/backups/backup.log 2>&1
```

### Step 22: Setup Monitoring

**Basic Monitoring with cron:**

```bash
# Create monitoring script
nano /home/deploy/monitor.sh
```

```bash
#!/bin/bash
if ! docker-compose -f /home/deploy/apps/vertex-transport/docker-compose.prod.yml ps | grep -q "Up"; then
    echo "Container down detected at $(date)" >> /home/deploy/monitor.log
    docker-compose -f /home/deploy/apps/vertex-transport/docker-compose.prod.yml up -d
fi
```

```bash
chmod +x /home/deploy/monitor.sh

# Run every 5 minutes
crontab -e
# Add:
*/5 * * * * /home/deploy/monitor.sh
```

**Advanced: UptimeRobot (Recommended)**

1. Sign up at https://uptimerobot.com/ (free tier)
2. Add monitor:
   - Type: HTTP(s)
   - URL: https://yourdomain.com/healthz
   - Interval: 5 minutes
   - Alert contacts: Your email

### Step 23: Log Rotation

```bash
# Configure Docker log rotation
sudo nano /etc/docker/daemon.json
```

```json
{
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "10m",
    "max-file": "3"
  }
}
```

```bash
sudo systemctl restart docker
docker-compose -f docker-compose.prod.yml up -d
```

---

## Maintenance & Updates

### Updating the Application

```bash
cd /home/deploy/apps/vertex-transport

# Pull latest code
git pull origin main

# Backup database before update
/home/deploy/backups/backup-db.sh

# Rebuild and restart
docker-compose -f docker-compose.prod.yml up -d --build

# Check logs for errors
docker-compose -f docker-compose.prod.yml logs -f
```

### Viewing Logs

```bash
# All services
docker-compose -f docker-compose.prod.yml logs -f

# Specific service
docker-compose -f docker-compose.prod.yml logs -f backend
docker-compose -f docker-compose.prod.yml logs -f frontend
docker-compose -f docker-compose.prod.yml logs -f postgres

# Last 100 lines
docker-compose -f docker-compose.prod.yml logs --tail=100
```

### Database Management

```bash
# Access PostgreSQL
docker exec -it postgres psql -U shipmentuser -d shipment_tracking_prod

# Backup database manually
docker exec postgres pg_dump -U shipmentuser shipment_tracking_prod > backup.sql

# Restore database
docker exec -i postgres psql -U shipmentuser shipment_tracking_prod < backup.sql
```

---

## Troubleshooting

### Common Issues

**1. Containers Won't Start**

```bash
# Check logs
docker-compose -f docker-compose.prod.yml logs

# Check disk space
df -h

# Check Docker disk usage
docker system df
```

**2. SSL Certificate Issues**

```bash
# Renew manually
sudo certbot renew --force-renewal

# Check certificate expiry
sudo certbot certificates
```

**3. Database Connection Errors**

```bash
# Check postgres is running
docker-compose -f docker-compose.prod.yml ps postgres

# Check database logs
docker-compose -f docker-compose.prod.yml logs postgres

# Verify credentials in .env match
```

**4. Frontend Can't Connect to Backend**

```bash
# Check nginx config
docker exec nginx nginx -t

# Check CORS settings in .env
# Check API_URL in frontend .env
```

**5. High Memory Usage**

```bash
# Check container stats
docker stats

# Restart containers
docker-compose -f docker-compose.prod.yml restart
```

### Performance Optimization

**1. Enable PostgreSQL Performance Tuning**

```bash
# Edit postgres config
docker exec -it postgres bash
nano /var/lib/postgresql/data/postgresql.conf

# Add/modify:
shared_buffers = 2GB              # 25% of RAM
effective_cache_size = 6GB        # 75% of RAM
maintenance_work_mem = 512MB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
```

**2. Add Redis for Caching (Optional)**

```yaml
# Add to docker-compose.prod.yml
redis:
  image: redis:7-alpine
  restart: unless-stopped
  volumes:
    - redis_data:/data
  healthcheck:
    test: ["CMD", "redis-cli", "ping"]
    interval: 10s
    timeout: 3s
    retries: 3
```

---

## Security Checklist

- [ ] SSH key-based authentication enabled
- [ ] Root login disabled
- [ ] Firewall (UFW) configured
- [ ] Fail2ban installed and configured
- [ ] HTTPS enabled with valid SSL certificate
- [ ] Auto-renewal configured for SSL
- [ ] Strong database password set
- [ ] Strong JWT secret configured
- [ ] CORS properly restricted to your domain
- [ ] Regular backups configured
- [ ] Monitoring setup
- [ ] Log rotation enabled
- [ ] Server updates scheduled
- [ ] .env file not committed to git

---

## Cost Estimation (Hetzner)

### Monthly Costs

- **Server CPX31**: €13.46/month
- **Backups (optional)**: +20% = €2.69/month
- **Total Hetzner**: ~€16.15/month

### Additional Costs

- **Domain**: $10-15/year (~€1/month)
- **Email (Resend)**: Free tier or $20/month
- **Total Monthly**: ~€17-35/month

---

## Support & Resources

### Hetzner Resources
- Documentation: https://docs.hetzner.com/
- Community: https://community.hetzner.com/
- Status Page: https://status.hetzner.com/

### Application Support
- GitHub Issues: [Your Repository]/issues
- Documentation: Check README.md

### Emergency Contacts
- Server down: Check Hetzner status
- Application errors: Check container logs
- SSL issues: Check Certbot logs

---

## Next Steps

After deployment:

1. **Test Everything**
   - Create test shipment
   - Test user registration
   - Verify emails are sending
   - Test admin functions

2. **Setup Monitoring**
   - Configure UptimeRobot
   - Add error tracking (Sentry)
   - Setup Google Analytics

3. **SEO Optimization**
   - Submit sitemap to Google Search Console
   - Verify site in Bing Webmaster Tools
   - Add structured data

4. **Performance Testing**
   - Run Lighthouse audit
   - Test with GTmetrix
   - Check SSL Labs rating

5. **Documentation**
   - Document any custom configurations
   - Create runbook for common tasks
   - Train team members

---

## Quick Reference Commands

```bash
# Application directory
cd /home/deploy/apps/vertex-transport

# Start all services
docker-compose -f docker-compose.prod.yml up -d

# Stop all services
docker-compose -f docker-compose.prod.yml down

# Restart specific service
docker-compose -f docker-compose.prod.yml restart nginx

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Update application
git pull && docker-compose -f docker-compose.prod.yml up -d --build

# Backup database
/home/deploy/backups/backup-db.sh

# Check SSL certificate
sudo certbot certificates

# Monitor containers
docker stats
```

---

**Congratulations! Your Vertex Transport application is now deployed on Hetzner Cloud and ready for production use! 🚀**
