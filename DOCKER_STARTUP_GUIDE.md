# Docker Startup Guide - Shipment Tracking System

## Phase 4 Complete - 100% Integration Ready!

All mock data has been removed and the system is fully integrated with real APIs. You're ready to test!

---

## Quick Start

### Option 1: Backend Only (For Testing)

```bash
cd "shipment-track-backend/docker"
docker-compose up -d
```

**What this starts:**
- PostgreSQL database (port 5432)
- Backend API (port 5000)
- PgAdmin (port 5050) - Database admin tool
- MailHog (ports 1025, 8025) - Email testing

**Access Points:**
- Backend API: http://localhost:5000
- Backend Health: http://localhost:5000/healthz/ready
- Swagger Docs: http://localhost:5000/swagger
- PgAdmin: http://localhost:5050
  - Email: `admin@admin.com`
  - Password: `admin`
- MailHog UI: http://localhost:8025

---

### Option 2: Full Stack with Nginx (Production-like)

```bash
cd "c:\Users\sajoo\OneDrive\Documents\yusef_project_(2)[1]\yusef project"
docker-compose -f docker-compose.prod.yml up -d
```

**What this starts:**
- Everything from Option 1
- Frontend (Next.js)
- Nginx reverse proxy (port 80)

**Access Points:**
- Frontend: http://localhost
- Backend API (via nginx): http://localhost/api
- Direct backend: http://localhost:5000

---

## Admin Credentials

Your admin account has been configured:

```
Email: rayvertex1@yahoo.com
Password: dLf!Ag22Wo1qZwK}
```

This account is seeded automatically when the backend starts for the first time.

---

## Step-by-Step Testing Guide

### 1. Start Backend

```bash
cd "shipment-track-backend/docker"
docker-compose up -d
```

Wait for services to start (about 30 seconds):

```bash
# Check if services are running
docker-compose ps

# Watch backend logs
docker-compose logs -f api
```

**Look for these lines in the logs:**
```
Application started successfully
Listening on: http://[::]:8080
Admin user seeded successfully
```

### 2. Test Backend Health

```bash
curl http://localhost:5000/healthz/ready
```

**Expected response:**
```json
{
  "status": "Healthy"
}
```

### 3. Test Admin Login (Backend Only)

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"rayvertex1@yahoo.com\",\"password\":\"dLf!Ag22Wo1qZwK}\"}"
```

**Expected response:**
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "eyJ...",
  "expiresIn": 900
}
```

If you see tokens, the backend is working correctly!

### 4. Start Frontend (Separate Terminal)

```bash
cd "shipment-track-frontend"
pnpm install  # First time only
pnpm dev
```

Frontend will start on: http://localhost:3000

### 5. Test Full Integration

1. **Open Browser**: http://localhost:3000
2. **Login**:
   - Email: `rayvertex1@yahoo.com`
   - Password: `dLf!Ag22Wo1qZwK}`
3. **Verify Dashboard** loads (admins only)
4. **Test Features**:
   - Create a shipment
   - Update shipment status
   - View shipment history
   - Public tracking (no login required)

---

## Testing Checklist

### Authentication ✅
- [ ] Login with admin credentials works
- [ ] Dashboard redirects after login
- [ ] Logout works
- [ ] Token refresh happens automatically (after 15 min)

### Shipments ✅
- [ ] List shipments loads (will be empty until you create shipments)
- [ ] Create new shipment works
- [ ] Edit shipment works
- [ ] Delete shipment works
- [ ] Add status update works
- [ ] View history works
- [ ] Pagination works (after adding 10+ shipments)
- [ ] Search/filter works

### Public Tracking ✅
- [ ] Can track shipment without login
- [ ] Shows status timeline (requires real status updates)
- [ ] Works with tracking numbers that exist in the database

### Users ✅
- [ ] List users loads (at least admin user)
- [ ] Create new user works
- [ ] Edit user works
- [ ] Delete user works
- [ ] Pagination works

### Dashboard ✅
- [ ] Statistics load correctly
- [ ] Recent shipments display
- [ ] Counts update after creating shipments

---

## Troubleshooting

### Backend won't start

**Check database connection:**
```bash
docker-compose logs postgres
```

**Recreate database:**
```bash
docker-compose down -v
docker-compose up -d
```

### Frontend can't connect to backend

**Check .env.local:**
```
NEXT_PUBLIC_API_URL=http://localhost:5000/api
API_INTERNAL_URL=http://localhost:5000/api
```

**Check CORS in backend logs:**
```bash
docker-compose logs api | grep CORS
```

### "401 Unauthorized" errors

**Token expired:**
- Logout and login again
- Token refresh should happen automatically

**Admin user not seeded:**
```bash
# Check backend logs for "Admin user seeded"
docker-compose logs api | grep "Admin"
```

### Database errors

**Reset database:**
```bash
cd shipment-track-backend/docker
docker-compose down -v
docker-compose up -d
```

**Remove legacy demo shipments:**
```bash
# Runs once; remove or edit infra/db/remove_demo_shipments.sql to target other tracking numbers
docker-compose -f docker-compose.prod.yml run --rm --profile maintenance db-cleanup
```

> New deployments skip demo shipment seeding outside development. Only run the cleanup if your database already contains the original VTX-202411-0001…0005 records.

**Connect with PgAdmin:**
1. Open: http://localhost:5050
2. Login: `admin@admin.com` / `admin`
3. Add Server:
   - Name: Shipment DB
   - Host: postgres
   - Port: 5432
   - Database: shipment_tracking
   - Username: shipmentuser
   - Password: `j5yNFuNnxSfANHBUj--CNeOt`

---

## Useful Commands

### View Logs
```bash
# All services
docker-compose logs -f

# Just backend
docker-compose logs -f api

# Just database
docker-compose logs -f postgres
```

### Stop Services
```bash
# Stop but keep data
docker-compose stop

# Stop and remove containers (keeps data)
docker-compose down

# Stop and remove everything including data
docker-compose down -v
```

### Rebuild After Code Changes
```bash
# Backend
docker-compose build api
docker-compose up -d api

# Full rebuild
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### Database Operations
```bash
# Backup database
docker-compose exec postgres pg_dump -U shipmentuser shipment_tracking > backup.sql

# Restore database
docker-compose exec -T postgres psql -U shipmentuser shipment_tracking < backup.sql
```

---

## Environment Configuration

### Backend (.env)
Located: `shipment-track-backend/docker/.env`

**Key Variables:**
- `POSTGRES_*`: Database credentials
- `JWT__SECRETKEY`: Token signing key
- `ADMINUSER__*`: Default admin account
- `CORS_ALLOWED_ORIGINS`: Frontend URL for CORS

### Frontend (.env.local)
Located: `shipment-track-frontend/.env.local`

**Key Variables:**
- `NEXT_PUBLIC_API_URL`: Backend API URL (client-side)
- `API_INTERNAL_URL`: Backend API URL (server-side)

---

## What's New in Phase 4 (100% Complete)

### ✅ All Mock Data Removed
- Users page now uses real backend API
- Settings page removed (no backend endpoints yet)
- All components use real data

### ✅ New Features Added
1. **User Management** - Full CRUD with backend
2. **Email Verification Page** - `/verify-email`
3. **Password Reset Page** - `/reset-password`
4. **Pagination** - Users page has pagination
5. **Loading States** - All operations show loading
6. **Error Handling** - Comprehensive error messages

### ✅ Services Created
- `user.service.ts` - User CRUD operations
- Type definitions for users
- Auth flow pages complete

---

## Next Steps (Phase 5)

After testing, you may want to:

1. **Add More Features**:
   - Bulk operations
   - Export to CSV/Excel
   - Advanced analytics charts
   - Real-time notifications

2. **Deploy to Production**:
   - Update admin credentials
   - Use production database
   - Configure domain/SSL
   - Set up CI/CD

3. **Performance Optimization**:
   - Add React Query for caching
   - Implement optimistic updates
   - Add service worker for offline

4. **Testing**:
   - Add unit tests
   - Add integration tests
   - Add E2E tests

---

## Support

If you encounter issues:

1. **Check logs first**: `docker-compose logs -f`
2. **Verify environment variables**: Check `.env` files
3. **Reset database**: `docker-compose down -v && docker-compose up -d`
4. **Rebuild containers**: `docker-compose build --no-cache`

---

## Success Indicators

You'll know everything is working when:

1. ✅ Backend health check returns "Healthy"
2. ✅ Admin login returns JWT tokens
3. ✅ Frontend loads without errors
4. ✅ Dashboard shows statistics (may be 0 initially)
5. ✅ Can create and manage shipments
6. ✅ Public tracking works
7. ✅ User management works

**Happy Testing! 🚀**
