# Vertex Transport Inc - Shipment Tracking System

A modern, production-ready shipment tracking application built with .NET 8 and Next.js 15.

## Features

- **Real-time Shipment Tracking**: Track shipments from origin to destination
- **Multi-port Support**: Manage shipments across multiple ports worldwide
- **User Management**: Role-based access control for admins and staff
- **Email Notifications**: Automated status updates via Resend or SMTP
- **RESTful API**: Clean Architecture with .NET 8
- **Modern UI**: Responsive interface built with Next.js and Tailwind CSS
- **Production Ready**: Docker-based deployment with SSL/TLS support

## Tech Stack

### Backend
- **.NET 8** - Modern, high-performance API
- **PostgreSQL 16** - Reliable relational database
- **Entity Framework Core** - ORM with migrations
- **JWT Authentication** - Secure token-based auth
- **Resend/SMTP** - Email notifications
- **Clean Architecture** - Maintainable and testable code

### Frontend
- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **Shadcn UI** - Beautiful component library

### Infrastructure
- **Docker & Docker Compose** - Containerized deployment
- **Nginx** - Reverse proxy and SSL termination
- **Let's Encrypt** - Free SSL certificates
- **Health Checks** - Service monitoring

## Quick Start

### Prerequisites
- Docker & Docker Compose
- Git
- Domain name (for production)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/shipment-tracking.git
   cd shipment-tracking
   ```

2. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

3. **Start the application**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - Frontend: http://localhost:3000
   - API: http://localhost:8080/api
   - Database: localhost:5432

### Production Deployment

See [DEPLOYMENT_README.md](DEPLOYMENT_README.md) for comprehensive deployment instructions.

## Environment Configuration

Copy `.env.example` to `.env` and configure the following:

### Required Variables

```bash
# Database
POSTGRES_PASSWORD=<strong-password>

# JWT Secret (generate with: openssl rand -base64 48)
JWT__SECRETKEY=<your-jwt-secret>

# Email Service (Resend recommended)
RESEND__APIKEY=<your-resend-api-key>
RESEND__FROMEMAIL=info@vertextransinc.com

# Admin Account
ADMINUSER__EMAIL=admin@vertextransinc.com
ADMINUSER__PASSWORD=<strong-admin-password>

# Domain
DOMAIN_NAME=vertextransinc.com
```

## Project Structure

```
.
├── shipment-track-backend/     # .NET 8 Web API
│   ├── src/
│   │   ├── ShipmentTracking.Domain/         # Domain entities
│   │   ├── ShipmentTracking.Application/    # Business logic
│   │   ├── ShipmentTracking.Infrastructure/ # Data access
│   │   └── ShipmentTracking.WebApi/         # API endpoints
│   └── docker/
├── shipment-track-frontend/    # Next.js 15 application
│   ├── app/                    # App Router pages
│   ├── components/             # React components
│   └── lib/                    # Utilities and services
├── infra/
│   ├── nginx/                  # Nginx configuration
│   ├── db/                     # Database scripts
│   └── scripts/                # Deployment scripts
├── docker-compose.prod.yml     # Production Docker config
└── .env.example                # Environment template
```

## API Documentation

### Authentication Endpoints
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh token

### Shipment Endpoints
- `GET /api/shipments` - List all shipments
- `GET /api/shipments/{id}` - Get shipment details
- `POST /api/shipments` - Create shipment
- `PUT /api/shipments/{id}` - Update shipment
- `POST /api/shipments/{id}/status` - Append status update

### User Management
- `GET /api/users` - List users (Admin only)
- `POST /api/users` - Create user (Admin only)
- `PUT /api/users/{id}` - Update user (Admin only)
- `DELETE /api/users/{id}` - Delete user (Admin only)

### Port Catalog
- `GET /api/ports` - List all ports
- `POST /api/ports` - Add new port (Admin only)

## Development

### Backend Development

```bash
cd shipment-track-backend/src/ShipmentTracking.WebApi
dotnet run
```

### Frontend Development

```bash
cd shipment-track-frontend
npm install
npm run dev
```

### Database Migrations

```bash
cd shipment-track-backend/src/ShipmentTracking.WebApi
dotnet ef migrations add MigrationName --project ../ShipmentTracking.Infrastructure
dotnet ef database update
```

## Security

- **Never commit** `.env` files
- Use strong passwords (minimum 32 characters)
- Rotate secrets every 90 days
- Enable HTTPS in production
- Regular security updates

### Security Checklist
- [ ] Strong database password set
- [ ] JWT secret generated (48+ characters)
- [ ] Admin password changed from default
- [ ] CORS origins configured
- [ ] SSL/TLS certificates installed
- [ ] Firewall rules configured
- [ ] Regular backups enabled

## Deployment

### Prerequisites
- Ubuntu 22.04 or later
- Docker and Docker Compose installed
- Domain DNS configured

### One-Command Deployment

```bash
# Clone and deploy
git clone https://github.com/yourusername/shipment-tracking.git
cd shipment-tracking
cp .env.example .env
# Edit .env with production values
docker-compose -f docker-compose.prod.yml up -d
```

See [DEPLOYMENT_README.md](DEPLOYMENT_README.md) for detailed instructions.

## Monitoring

### Health Checks
- Backend: `http://your-domain/api/healthz/ready`
- Frontend: `http://your-domain/`

### Logs
```bash
# View all logs
docker-compose -f docker-compose.prod.yml logs -f

# Service-specific logs
docker-compose -f docker-compose.prod.yml logs -f backend
docker-compose -f docker-compose.prod.yml logs -f frontend
```

## Backup and Restore

### Backup Database
```bash
docker-compose -f docker-compose.prod.yml exec postgres pg_dump \
  -U shipmentuser -d shipment_tracking_prod > backup.sql
```

### Restore Database
```bash
cat backup.sql | docker-compose -f docker-compose.prod.yml exec -T postgres \
  psql -U shipmentuser -d shipment_tracking_prod
```

## Troubleshooting

### Common Issues

**Database connection failed**
- Check PostgreSQL is running: `docker-compose ps postgres`
- Verify credentials in `.env`

**Frontend cannot connect to API**
- Check `NEXT_PUBLIC_API_URL` in `.env`
- Verify CORS settings

**SSL certificate issues**
- Ensure DNS is properly configured
- Check Let's Encrypt rate limits
- Verify domain in `.env`

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit changes: `git commit -am 'Add my feature'`
4. Push to branch: `git push origin feature/my-feature`
5. Create a Pull Request

## License

This project is proprietary software. All rights reserved.

## Support

For issues and questions:
- Email: support@vertextransinc.com
- Website: https://vertextransinc.com

## Acknowledgments

Built with modern technologies for reliable shipment tracking.
