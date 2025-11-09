# Vertex Transport - Production Deployment Summary

Quick reference guide for production deployment on Hetzner Cloud.

## 🚀 Quick Start

### Hosting Recommendation: Hetzner Cloud

**Recommended VPS:** CPX31 (€13.46/month)
- 4 vCPUs
- 8 GB RAM
- 160 GB SSD
- Perfect for production traffic

**Location:** Choose closest to your users
- 🇪🇺 Nuremberg, Germany (EU)
- 🇪🇺 Helsinki, Finland (EU)
- 🇺🇸 Ashburn, VA (US)

## 📦 Architecture

```
Internet
    ↓
Hetzner Cloud VPS (Ubuntu 22.04)
    ↓
Docker Containers:
    ├── Nginx (Reverse Proxy + SSL)
    ├── Next.js Frontend (Port 3000)
    ├── .NET Backend API (Port 8080)
    └── PostgreSQL Database (Port 5432)
```

## 🔧 What's Been Done (Production Ready Improvements)

### ✅ Security Enhancements
- Removed TypeScript build error ignoring
- Added environment variable validation (fails on missing production vars)
- **Configured HTTPS with automatic HTTP → HTTPS redirect (port 80 → 443)**
- **Production-ready nginx with SSL/TLS 1.2 + 1.3 support**
- **Enhanced security headers: HSTS, CSP, X-Frame-Options, X-Content-Type-Options, etc.**
- **Diffie-Hellman parameters for perfect forward secrecy**
- **OCSP stapling enabled for faster SSL verification**
- **Rate limiting: 10 req/s for API, 30 req/s for general traffic**
- Enabled gzip compression for all text content
- Created automated SSL setup and renewal scripts
- Docker volumes configured for SSL certificates and DH parameters

### ✅ SEO Optimization
- Created robots.txt
- Implemented sitemap.xml generation
- Added comprehensive Open Graph and Twitter Card metadata
- Configured PWA manifest
- Added SEO-friendly page titles and descriptions
- Enabled image optimization
- Added structured data schema placeholders

### ✅ Production Configuration
- Created `.env.example` with all required variables including `DOMAIN_NAME`
- Configured production appsettings for backend
- **HTTPS redirect enabled (HTTP 301 → HTTPS)**
- **nginx uses environment variable substitution for domain names**
- **Exposed ports 80 and 443 in docker-compose**
- Optimized logging for production
- Set up health check endpoints for all services

### ✅ Deployment Automation
- **Created automated SSL setup script (`infra/scripts/setup-ssl.sh`)**
- **Created SSL renewal script (`infra/scripts/renew-ssl.sh`)**
- **Created deployment script (`infra/scripts/deploy.sh`)**
- Complete Hetzner deployment guide with SSL instructions
- Production checklist with HTTPS verification steps
- Backup and monitoring procedures
- Troubleshooting guides

## 📋 Deployment Steps (Summary)

### 1. Server Setup (30 minutes)
```bash
# Create Hetzner CPX31 server
# Ubuntu 22.04, enable backups
# Configure firewall: SSH (22), HTTP (80), HTTPS (443)

# Install Docker & Docker Compose
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
```

### 2. Configure Domain (15 minutes)
```
A Record:     @    →  YOUR_SERVER_IP
A Record:     www  →  YOUR_SERVER_IP
```

### 3. Deploy Application (20 minutes)
```bash
# Clone repository
git clone https://github.com/YOUR_USERNAME/vertex-transport.git
cd vertex-transport

# Configure environment
cp .env.example .env
nano .env  # Fill in your values

# Deploy
docker-compose -f docker-compose.prod.yml up -d --build
```

### 4. Setup SSL (10 minutes - Automated!)
```bash
# Run automated SSL setup script
chmod +x infra/scripts/setup-ssl.sh
sudo ./infra/scripts/setup-ssl.sh

# The script will:
# - Install Certbot
# - Generate SSL certificates
# - Create DH parameters for enhanced security
# - Configure auto-renewal
# - Restart nginx with HTTPS enabled
```

**Or Manual Setup:**
```bash
# Install Certbot
sudo apt install certbot

# Get certificate
sudo certbot certonly --standalone \
  -d yourdomain.com \
  -d www.yourdomain.com

# Generate DH parameters (takes 5-10 minutes)
sudo openssl dhparam -out /etc/nginx/dhparam.pem 4096

# Add domain to .env
echo "DOMAIN_NAME=yourdomain.com" >> .env

# Restart nginx
docker-compose -f docker-compose.prod.yml restart nginx
```

### 5. Configure Backups & Monitoring (20 minutes)
```bash
# Setup daily database backups (see DEPLOYMENT_HETZNER.md)
# Configure UptimeRobot for monitoring
# Setup SSL auto-renewal cron job
```

**Total Time:** ~2 hours

## 🔐 Critical Security Steps

### Before Deployment:

1. **Remove .env from Git**
   ```bash
   # If .env is in git history, clean it:
   git filter-branch --force --index-filter \
     "git rm --cached --ignore-unmatch .env" \
     --prune-empty --tag-name-filter cat -- --all
   ```

2. **Generate Strong Secrets**
   ```bash
   # JWT Secret (min 32 chars)
   openssl rand -base64 48

   # Database Password
   openssl rand -base64 32
   ```

3. **Update Environment Variables**
   - Use secrets manager in production (recommended)
   - Or use strong, unique passwords in .env
   - Never commit .env to git

## 📊 Required Environment Variables

### Backend
```bash
POSTGRES_PASSWORD=<strong-password>
JWT_SECRET=<minimum-32-chars>
RESEND_API_KEY=<your-api-key>
CORS_ALLOWED_ORIGINS=https://yourdomain.com
```

### Frontend
```bash
NEXT_PUBLIC_API_URL=https://yourdomain.com/api
NEXT_PUBLIC_SITE_URL=https://yourdomain.com
NODE_ENV=production
```

See `.env.example` for complete list.

## 📈 Monitoring Setup

### Recommended Free Tools:

1. **UptimeRobot** (uptime monitoring)
   - Monitor: https://yourdomain.com/healthz
   - Alerts via email/SMS

2. **Google Search Console** (SEO)
   - Submit sitemap: https://yourdomain.com/sitemap.xml
   - Monitor indexing and performance

3. **SSL Labs** (security)
   - Test SSL: https://www.ssllabs.com/ssltest/
   - Aim for A+ rating

## 🆘 Quick Troubleshooting

### Site Not Loading
```bash
# Check containers
docker-compose -f docker-compose.prod.yml ps

# Check logs
docker-compose -f docker-compose.prod.yml logs -f

# Restart all
docker-compose -f docker-compose.prod.yml restart
```

### SSL Certificate Issues
```bash
# Check certificates
sudo certbot certificates

# Renew manually
sudo certbot renew --force-renewal
```

### Database Connection Errors
```bash
# Check postgres
docker-compose -f docker-compose.prod.yml logs postgres

# Verify environment variables
cat .env | grep POSTGRES
```

## 💰 Cost Breakdown

### Monthly Costs:
- **Hetzner CPX31:** €13.46/month
- **Backups:** +€2.69/month (optional)
- **Domain:** ~€1/month ($12/year)
- **Email (Resend):** Free tier or $20/month
- **Total:** ~€17-38/month

### One-Time Costs:
- Domain registration: $10-15/year
- Initial setup time: ~2 hours

## 📚 Full Documentation

- **[DEPLOYMENT_HETZNER.md](./DEPLOYMENT_HETZNER.md)** - Complete deployment guide
- **[PRODUCTION_CHECKLIST.md](./PRODUCTION_CHECKLIST.md)** - Pre-deployment checklist
- **[.env.example](./.env.example)** - Environment variables reference

## ✨ Production Features

### SEO Optimized
- ✅ Comprehensive meta tags (Open Graph, Twitter Cards)
- ✅ Sitemap.xml auto-generation
- ✅ Robots.txt configured
- ✅ PWA manifest
- ✅ Structured data ready
- ✅ Image optimization enabled

### Security Hardened
- ✅ HTTPS/SSL configured
- ✅ Security headers (HSTS, CSP, X-Frame-Options, etc.)
- ✅ Rate limiting (10 req/s for API, 30 req/s general)
- ✅ CORS restricted to production domain
- ✅ Environment variable validation
- ✅ Production logging configured

### Performance Optimized
- ✅ Gzip compression enabled
- ✅ Image optimization (AVIF/WebP)
- ✅ Docker multi-stage builds
- ✅ Next.js standalone output
- ✅ Static asset caching
- ✅ Database connection pooling

### Operational
- ✅ Health check endpoints
- ✅ Automated database backups
- ✅ SSL auto-renewal
- ✅ Log rotation
- ✅ Monitoring ready
- ✅ Rollback procedures documented

## 🎯 Next Steps After Deployment

1. **Test Everything**
   - Create test shipment
   - Test user flows
   - Verify emails work
   - Test on mobile

2. **SEO Setup**
   - Add to Google Search Console
   - Add to Bing Webmaster Tools
   - Submit sitemap

3. **Monitoring**
   - Setup UptimeRobot
   - Configure alerts
   - Add error tracking (Sentry)

4. **Optimization**
   - Run Lighthouse audit
   - Test with GTmetrix
   - Check SSL Labs rating
   - Optimize based on results

## 🔗 Useful Links

- **Hetzner Console:** https://console.hetzner.cloud/
- **Hetzner Docs:** https://docs.hetzner.com/
- **Let's Encrypt:** https://letsencrypt.org/
- **SSL Labs Test:** https://www.ssllabs.com/ssltest/
- **UptimeRobot:** https://uptimerobot.com/
- **Google Search Console:** https://search.google.com/search-console/

## 🏆 Success Criteria

Your deployment is successful when:

- [ ] Site accessible via HTTPS with valid certificate
- [ ] All pages load correctly
- [ ] Users can register and login
- [ ] Shipment tracking works
- [ ] Admin panel accessible
- [ ] Email notifications working
- [ ] Lighthouse score > 90
- [ ] SSL Labs grade A or A+
- [ ] Uptime monitoring configured
- [ ] Backups running daily

---

## 💡 Pro Tips

1. **Use staging environment** - Test on a smaller Hetzner server first
2. **Enable backups** - Hetzner snapshots are worth the 20% extra cost
3. **Monitor from day 1** - Catch issues before users do
4. **Document everything** - Your future self will thank you
5. **Regular updates** - Keep dependencies and OS updated monthly

---

**Need Help?**

- Check [DEPLOYMENT_HETZNER.md](./DEPLOYMENT_HETZNER.md) for detailed steps
- Review [PRODUCTION_CHECKLIST.md](./PRODUCTION_CHECKLIST.md) before deploying
- Open an issue in the repository
- Contact Hetzner support for server issues

**Ready to deploy? Follow the complete guide in DEPLOYMENT_HETZNER.md** 🚀
