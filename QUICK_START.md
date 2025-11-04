# Quick Start - Shipment Tracking System

## TL;DR - Start in 30 Seconds

```bash
# 1. Start backend
cd "shipment-track-backend/docker"
docker-compose up -d

# 2. Wait 30 seconds, then start frontend (new terminal)
cd "../shipment-track-frontend"
pnpm dev

# 3. Open browser
# http://localhost:3000
```

**Login:**
- Email: `rayvertex1@yahoo.com`
- Password: `dLf!Ag22Wo1qZwK}`

---

## What You Get

✅ **Backend** on port 5000
✅ **Frontend** on port 3000
✅ **Database** PostgreSQL
✅ **Email Testing** MailHog on port 8025
✅ **Database Admin** PgAdmin on port 5050

---

## Test These Features

### Must Test:
1. **Login** → Should redirect to dashboard
2. **Dashboard** → Should show stats (0 initially)
3. **Create Shipment** → Fill form and save
4. **View Shipments** → Should see your shipment
5. **Add Status** → Update shipment status
6. **Public Track** → Track without login
7. **Users** → Manage users (at least admin visible)

### Nice to Test:
- Edit shipment
- Delete shipment
- View history
- Search/filter
- Pagination (after 10+ items)
- Logout/login again

---

## Quick Commands

### Check if Running
```bash
docker-compose ps
```

### View Logs
```bash
docker-compose logs -f api
```

### Stop Everything
```bash
docker-compose down
```

### Reset Database
```bash
docker-compose down -v
docker-compose up -d
```

---

## Troubleshooting 1-Liners

| Problem | Solution |
|---------|----------|
| Backend won't start | `docker-compose down -v && docker-compose up -d` |
| 401 errors | Logout and login again |
| Can't connect | Check `.env.local` has `http://localhost:5000/api` |
| Database errors | `docker-compose restart postgres` |
| CORS errors | Check backend logs: `docker-compose logs api \| grep CORS` |

---

## Success Indicators

✅ Backend: `curl http://localhost:5000/healthz/ready` returns "Healthy"
✅ Login returns JWT tokens
✅ Dashboard loads without console errors
✅ Can create and view shipments
✅ Public tracking works

---

## Full Details

For detailed instructions, see [DOCKER_STARTUP_GUIDE.md](DOCKER_STARTUP_GUIDE.md)

For completion report, see [PHASE_4_COMPLETION_REPORT.md](PHASE_4_COMPLETION_REPORT.md)

---

**Happy Testing! 🚀**
