# N5 Permissions Challenge

Sistema de gestión de permisos de empleados. Backend en .NET 8, frontend en React + TypeScript.

## Inicio rápido con Docker

```bash
docker-compose up --build
```

Accesos:

- App: http://localhost:3000
- API/Swagger: http://localhost:5290/swagger
- Kibana: http://localhost:5601
- Kafka UI: http://localhost:8080

```bash
docker-compose down      # detener
docker-compose down -v   # detener y borrar datos
```

## Inicio en desarrollo local

Requisitos: .NET 8, Node.js 22+, Docker

```bash
# 1. Infraestructura
docker-compose up -d sqlserver elasticsearch kafka zookeeper kibana kafka-ui

# 2. Backend
cd backend/src/N5.Permissions.API
dotnet run

# 3. Frontend (en otra terminal)
cd frontend
npm install && npm run dev
```

## Servicios

Accesos:

- App: http://localhost:5173
- API/Swagger: http://localhost:5290/swagger
- Kibana: http://localhost:5601
- Kafka UI: http://localhost:8080

| Servicio      | Puerto | Credenciales      |
| ------------- | ------ | ----------------- |
| Frontend      | 5173   | -                 |
| API           | 5290   | -                 |
| SQL Server    | 1433   | sa / testPass-123 |
| Elasticsearch | 9200   | -                 |
| Kafka         | 9092   | -                 |
| Kibana        | 5601   | -                 |
| Kafka UI      | 8080   | -                 |

## API

| Método | Ruta                  | Descripción       |
| ------ | --------------------- | ----------------- |
| GET    | /api/permissions      | Listar permisos   |
| POST   | /api/permissions      | Crear permiso     |
| PUT    | /api/permissions/{id} | Modificar permiso |

**Crear:**

```json
POST /api/permissions
{
  "nombreEmpleado": "Juan",
  "apellidoEmpleado": "Pérez",
  "tipoPermiso": 1,
  "fechaPermiso": "2024-12-25"
}
```

**Modificar** (campos opcionales):

```json
PUT /api/permissions/1
{
  "nombreEmpleado": "Juan Carlos"
}
```

**Tipos de permiso:** 1=Enfermedad, 2=Tramite, 3=Mudanza, 4=Vacaciones

## Tests

```bash
cd backend && dotnet test
```

## Arquitectura

```
Frontend (React 19 + TS + MUI)
    │
    ▼
Web API (.NET 8)
├── CQRS (Commands/Queries separados)
├── Repository + Unit of Work
└── Clean Architecture (4 capas)
    │
    ├──► SQL Server (EF Core)
    ├──► Elasticsearch (indexación)
    └──► Kafka (eventos)
```

## Estructura

```
backend/
├── src/
│   ├── N5.Permissions.API/            # Controllers, middleware
│   ├── N5.Permissions.Application/    # Handlers, DTOs
│   ├── N5.Permissions.Domain/         # Entidades, interfaces
│   └── N5.Permissions.Infrastructure/ # Repos, EF, Kafka, ES
└── tests/
    ├── N5.Permissions.UnitTests/
    └── N5.Permissions.IntegrationTests/

frontend/src/
├── api/         # Cliente axios
├── components/  # React components
├── types/       # TypeScript interfaces
└── theme/       # MUI theme
```

## Notas

- La DB se crea automáticamente al iniciar (migraciones)
- Kafka publica en `permissions-operations` en cada operación
- Elasticsearch indexa los permisos solo en POST y PUT. El GET no indexa porque es una operación de lectura que no modifica datos, y como los documentos tienen la misma estructura que la
  tabla, reindexar en cada consulta sobrescribiría información sin agregar valor
- El `.env` está en el repo por ser un challenge de evaluación
