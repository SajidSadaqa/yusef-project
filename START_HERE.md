# START HERE - Quick Start Guide

## ⚡ RECOMMENDED: Easiest Way to Test

### Option 1: Backend in Docker, Frontend Local (FASTEST & EASIEST)

This is the most reliable way to test:

```bash
# 1. Start backend (in first terminal)
cd "shipment-track-backend/docker"
docker-compose up -d

# 2. Wait 30 seconds, then start frontend (in second terminal)
cd "../../shipment-track-frontend"
pnpm dev
```

**Access:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000

---

### Option 2: Full Production Stack (Docker Everything)

**IMPORTANT:** The production docker-compose has been fixed. Try it now:

```bash
# From project root
docker-compose -f docker-compose.prod.yml up -d
```

**What was fixed:**
1. ✅ Created `.env` file in project root with all variables
2. ✅ Fixed frontend Dockerfile to handle lockfile mismatch
3. ✅ Updated docker-compose.prod.yml to use root `.env` file

**Access:**
- Everything: http://localhost (via nginx)
- Backend direct: http://localhost:5000

---

## Login Credentials

```
Email: rayvertex1@yahoo.com
Password: dLf!Ag22Wo1qZwK}
```

---

## If You Get Errors

### "Variable not set" errors
✅ **FIXED** - `.env` file now exists in project root

### Frontend build fails with lockfile error
✅ **FIXED** - Dockerfile now uses `--no-frozen-lockfile`

### Other issues
```bash
# Clean rebuild
docker-compose -f docker-compose.prod.yml down -v
docker-compose -f docker-compose.prod.yml build --no-cache
docker-compose -f docker-compose.prod.yml up -d
```

---

## My Recommendation

**Use Option 1** (Backend in Docker, Frontend with pnpm dev):
- Faster to start
- Easier to see frontend errors
- Hot reload works for frontend development
- Backend is containerized and stable

**Use Option 2** when:
- Testing production deployment
- Need nginx reverse proxy
- Want to test full containerized stack
- Preparing for actual deployment

---

## Next Steps

1. Login to http://localhost:3000
2. Test creating a shipment
3. Test tracking (public, no login needed)
4. Check users page
5. Try all CRUD operations

See [DOCKER_STARTUP_GUIDE.md](DOCKER_STARTUP_GUIDE.md) for detailed testing checklist.

---

**Quick Health Check:**
```bash
# Backend health
curl http://localhost:5000/healthz/ready

# Test login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"rayvertex1@yahoo.com\",\"password\":\"dLf!Ag22Wo1qZwK}\"}"
```

Good luck! 🚀
