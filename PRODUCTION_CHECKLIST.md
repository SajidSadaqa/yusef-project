# Production Deployment Checklist

Complete checklist before deploying Vertex Transport to production.

## Pre-Deployment Checklist

### Security

- [ ] Remove `.env` file from git repository
- [ ] Rotate all secrets (JWT keys, database passwords, API keys)
- [ ] Generate strong JWT secret (min 32 characters): `openssl rand -base64 48`
- [ ] Generate strong database password: `openssl rand -base64 32`
- [ ] Configure CORS to only allow your production domain
- [ ] Enable HTTPS redirect in nginx configuration (HTTP 80 → HTTPS 443)
- [ ] Verify HTTPS server block is enabled in nginx
- [ ] Generate Diffie-Hellman parameters: `openssl dhparam -out /etc/nginx/dhparam.pem 4096`
- [ ] Verify security headers are configured in nginx (HSTS, CSP, X-Frame-Options, etc.)
- [ ] Test SSL certificate validity and auto-renewal
- [ ] Configure firewall (UFW) on server - allow only SSH, HTTP, HTTPS
- [ ] Disable root SSH login (edit `/etc/ssh/sshd_config`)
- [ ] Setup SSH key-based authentication only (disable password auth)
- [ ] Install and configure fail2ban for SSH protection
- [ ] Set DOMAIN_NAME environment variable in .env

### Frontend

- [ ] Fix all TypeScript build errors
- [ ] Remove all console.log statements
- [ ] Configure proper environment variables (no localhost fallbacks)
- [ ] Add comprehensive SEO metadata
- [ ] Create robots.txt
- [ ] Create sitemap.xml
- [ ] Add favicon and touch icons
- [ ] Enable image optimization
- [ ] Test all pages load correctly
- [ ] Test responsive design on mobile/tablet
- [ ] Run Lighthouse audit (aim for 90+ scores)
- [ ] Verify all images have alt tags
- [ ] Test error boundaries work

### Backend

- [ ] Verify database migrations are up to date
- [ ] Test all API endpoints
- [ ] Configure production logging (Warning level)
- [ ] Setup email service (Resend/SMTP)
- [ ] Test email sending works
- [ ] Verify rate limiting is configured
- [ ] Test JWT authentication
- [ ] Test authorization (Admin/Staff/Customer roles)
- [ ] Verify health check endpoint works
- [ ] Configure CORS for production domain only
- [ ] Test database connection pooling

### Database

- [ ] Backup current database
- [ ] Test database restore procedure
- [ ] Verify migrations run automatically
- [ ] Setup automated daily backups
- [ ] Configure backup retention policy
- [ ] Test backup recovery process
- [ ] Optimize PostgreSQL settings for production
- [ ] Verify database password is strong

### Infrastructure

- [ ] Server has sufficient resources (8GB RAM recommended)
- [ ] Domain DNS is configured correctly (A and AAAA records)
- [ ] SSL certificates obtained and installed via `./infra/scripts/setup-ssl.sh`
- [ ] SSL auto-renewal configured (cron job runs twice daily)
- [ ] nginx compression enabled (gzip)
- [ ] nginx security headers configured (HSTS, CSP, X-Frame-Options, etc.)
- [ ] nginx rate limiting configured (API: 10r/s, General: 30r/s)
- [ ] Docker and Docker Compose installed on server
- [ ] All containers have health checks configured
- [ ] Docker log rotation configured (max-size: 10m, max-file: 3)
- [ ] Port 80 and 443 exposed in docker-compose for nginx
- [ ] SSL certificate volumes mounted in nginx container
- [ ] DH parameters mounted in nginx container
- [ ] Monitoring setup (UptimeRobot/similar)

### Testing

- [ ] Test user registration flow
- [ ] Test user login/logout
- [ ] Test password reset (if implemented)
- [ ] Test shipment creation
- [ ] Test shipment tracking
- [ ] Test admin functions (users, ports, shipments)
- [ ] Test file uploads (if any)
- [ ] Test email notifications
- [ ] Load test with expected traffic
- [ ] Test on different browsers (Chrome, Firefox, Safari, Edge)
- [ ] Test on mobile devices
- [ ] Test with slow network connection

### SEO & Performance

- [ ] Submit sitemap to Google Search Console
- [ ] Submit sitemap to Bing Webmaster Tools
- [ ] Verify Google Analytics (if using)
- [ ] Test page load speed (< 3 seconds)
- [ ] Verify Core Web Vitals pass
- [ ] Test SSL rating (SSL Labs A+ grade)
- [ ] Configure caching headers
- [ ] Enable CDN for static assets (optional)
- [ ] Compress images
- [ ] Minify CSS/JS (Next.js does this automatically)

### Documentation

- [ ] README.md is up to date
- [ ] Environment variables documented
- [ ] Deployment process documented
- [ ] Backup/restore process documented
- [ ] Common troubleshooting steps documented
- [ ] API documentation available (Swagger UI)
- [ ] User guide created (if needed)

### Monitoring & Analytics

- [ ] Error tracking setup (Sentry recommended)
- [ ] Uptime monitoring configured
- [ ] Server metrics monitoring
- [ ] Database metrics monitoring
- [ ] Log aggregation configured
- [ ] Alert notifications configured
- [ ] Analytics tracking installed

### Legal & Compliance

- [ ] Privacy Policy page created
- [ ] Terms of Service page created
- [ ] Cookie consent (if using cookies)
- [ ] GDPR compliance verified (if serving EU)
- [ ] Data retention policy defined

## Post-Deployment Checklist

### Immediate (Within 1 hour)

- [ ] Verify site is accessible at production URL
- [ ] Test HTTPS is working (green padlock)
- [ ] Test main user flows (login, tracking, admin)
- [ ] Check application logs for errors
- [ ] Verify database connections are healthy
- [ ] Test email sending in production
- [ ] Verify monitoring is reporting
- [ ] Check SSL certificate expiry date

### Day 1

- [ ] Monitor error rates
- [ ] Monitor performance metrics
- [ ] Review server resource usage
- [ ] Check backup completed successfully
- [ ] Test all critical features again
- [ ] Gather initial user feedback
- [ ] Monitor email delivery rates

### Week 1

- [ ] Review all monitoring alerts
- [ ] Analyze performance bottlenecks
- [ ] Review error logs and fix issues
- [ ] Optimize database queries if needed
- [ ] Check SEO indexing status
- [ ] Review user analytics
- [ ] Plan first performance improvements

### Month 1

- [ ] Security audit
- [ ] Performance review
- [ ] Backup restoration test
- [ ] Review and rotate secrets
- [ ] Update dependencies
- [ ] Plan feature roadmap based on feedback
- [ ] Review and optimize costs

## Critical Issues - Do Not Deploy If:

🔴 **CRITICAL - Must Fix Before Deployment:**

- [ ] `.env` file is in git repository
- [ ] Using default/weak passwords
- [ ] TypeScript build has errors
- [ ] SSL certificate not configured
- [ ] CORS allows all origins (*)
- [ ] No backups configured
- [ ] No monitoring setup
- [ ] Console.logs expose sensitive data
- [ ] Environment variables hardcode localhost

## Rollback Plan

If deployment fails:

1. **Immediate Actions:**
   ```bash
   # Stop new deployment
   docker-compose -f docker-compose.prod.yml down

   # Restore previous version
   git checkout <previous-commit>
   docker-compose -f docker-compose.prod.yml up -d --build

   # Restore database if needed
   docker exec -i postgres psql -U shipmentuser shipment_tracking_prod < backup.sql
   ```

2. **Communication:**
   - Notify users of temporary downtime
   - Post status update
   - Communicate ETA for fix

3. **Investigation:**
   - Collect error logs
   - Identify root cause
   - Create fix plan
   - Test in staging

4. **Re-deployment:**
   - Fix issues
   - Test thoroughly
   - Deploy during low-traffic period
   - Monitor closely

## Emergency Contacts

- **Hetzner Support**: https://console.hetzner.cloud/
- **Domain Registrar**: [Your registrar support]
- **Email Service (Resend)**: support@resend.com
- **SSL (Let's Encrypt)**: https://community.letsencrypt.org/

## Useful Commands

```bash
# Check all services
docker-compose -f docker-compose.prod.yml ps

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Restart service
docker-compose -f docker-compose.prod.yml restart [service]

# Check SSL
sudo certbot certificates

# Database backup
docker exec postgres pg_dump -U shipmentuser shipment_tracking_prod | gzip > backup.sql.gz

# Server stats
htop
df -h
docker stats
```

## Sign-Off

Deployment completed by: ________________

Date: ________________

All checklist items verified: ☑

Production URL: ________________

Monitoring dashboard: ________________

---

**Remember:** Better to delay deployment than to deploy with critical issues!
