# N5 Permissions Challenge

Aplicación fullstack para gestión de permisos de empleados. Backend en .NET 8 con SQL Server, Elasticsearch y Kafka. Frontend en React + TypeScript con Material-UI.

## Requisitos

- .NET SDK 8.0+
- Node.js 22+ (LTS)
- Docker Desktop

## Levantar servicios

```bash
docker-compose up -d
```

Esto levanta SQL Server, Elasticsearch, Kafka, Kibana y Kafka UI.

## Ejecutar Backend

```bash
cd backend/src/N5.Permissions.API
dotnet run
```

La API corre en `http://localhost:5290`. Swagger disponible en `/swagger`.

## Ejecutar Frontend

```bash
cd frontend
npm install
npm run dev
```

El frontend corre en `http://localhost:5173` (puerto por defecto de Vite).

**Nota:** Se deja el .env en repositorio por ser un challenge

## Endpoints

| Método | Ruta                  | Descripción       |
| ------ | --------------------- | ----------------- |
| POST   | /api/permissions      | Crear permiso     |
| PUT    | /api/permissions/{id} | Modificar permiso |
| GET    | /api/permissions      | Listar permisos   |

### Crear permiso

```json
POST /api/permissions
{
  "nombreEmpleado": "Juan",
  "apellidoEmpleado": "Pérez",
  "tipoPermiso": 1,
  "fechaPermiso": "2024-12-25"
}
```

### Modificar permiso

```json
PUT /api/permissions/1
{
  "nombreEmpleado": "Juan Carlos",
  "tipoPermiso": 2
}
```

Todos los campos son opcionales en la modificación.

## Tipos de permiso

| ID  | Descripción |
| --- | ----------- |
| 1   | Enfermedad  |
| 2   | Tramite     |
| 3   | Mudanza     |
| 4   | Vacaciones  |

## Tests

```bash
cd backend
dotnet test
```

## Tecnologías

### Backend

- .NET 8
- Entity Framework Core
- SQL Server
- Elasticsearch (NEST)
- Apache Kafka (Confluent.Kafka)
- AutoMapper
- xUnit (tests)

### Frontend

- React 19
- TypeScript
- Vite
- Material-UI (MUI)
- Axios

## Estructura

```
├── backend/
│   ├── src/
│   │   ├── N5.Permissions.API/           # Controllers y configuración
│   │   ├── N5.Permissions.Application/   # Handlers y DTOs
│   │   ├── N5.Permissions.Domain/        # Entidades e interfaces
│   │   └── N5.Permissions.Infrastructure/# Repositorios y servicios externos
│   └── tests/
│       ├── N5.Permissions.UnitTests/
│       └── N5.Permissions.IntegrationTests/
└── frontend/
    ├── src/
    │   ├── api/                          # Cliente HTTP (axios)
    │   ├── components/                   # Componentes React
    │   ├── types/                        # Interfaces TypeScript
    │   └── theme/                        # Tema Material-UI
    └── public/
```

## Servicios Docker

| Servicio      | Puerto | Credenciales      |
| ------------- | ------ | ----------------- |
| SQL Server    | 1433   | sa / testPass-123 |
| Elasticsearch | 9200   | -                 |
| Kafka         | 9092   | -                 |
| Kibana        | 5601   | -                 |
| Kafka UI      | 8080   | -                 |

## Herramientas visuales

- **Kibana** (http://localhost:5601): Para ver los permisos indexados en Elasticsearch. Crear un Data View con el patrón `permissions`.
- **Kafka UI** (http://localhost:8080): Para ver los mensajes en el topic `permissions-operations`.

## Funcionalidades Frontend

- Listado de permisos en tabla
- Crear nuevo permiso (modal)
- Editar permiso existente (modal)
- Interfaz responsive con Material-UI
- Manejo de estados de carga y errores

## Notas

- La base de datos se crea automáticamente al iniciar la API (migraciones automáticas)
- Kafka publica eventos en cada operación (request, modify, get)
- Elasticsearch indexa los permisos solo en POST y PUT. El GET no indexa porque es una operación de lectura que no modifica datos, y como los documentos tienen la misma estructura que la tabla, reindexar en cada consulta sobrescribiría información sin agregar valor
