# Guía para probar `GET /api/Citas/agenda`

## 1. Requisitos previos

### Autenticación (obligatoria)
- **Rol**: Solo psicólogos. El JWT debe incluir el rol `Psicologo`.
- **Token**: Bearer en header `Authorization: Bearer {accessToken}`.

### Cómo obtener el token

**Paso 1 – Login psicólogo**
```http
POST {{baseUrl}}/api/Autenticacion/ValidarPsicologo
Content-Type: application/json

{
  "correoPersonal": "tu_correo@ejemplo.com",
  "password": "tu_contraseña"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "psicologoId": 1,
  "tokenType": "Bearer"
}
```

Copia el valor de `accessToken` para usarlo en el siguiente request.

---

## 2. Llamada al endpoint

```http
GET {{baseUrl}}/api/Citas/agenda?desde=2026-03-16&hasta=2026-03-22
Authorization: Bearer {accessToken}
```

### Parámetros query (opcionales)

| Parámetro | Tipo | Formato | Descripción |
|-----------|------|---------|-------------|
| `desde` | string | `YYYY-MM-DD` | Fecha inicio del rango |
| `hasta` | string | `YYYY-MM-DD` | Fecha fin del rango |

Si no se envían:
- `desde` = 7 días atrás
- `hasta` = 30 días adelante  
- Límite máximo: 90 días entre `desde` y `hasta`.

---

## 3. Respuestas posibles

### 200 OK – Éxito

Devuelve un **array de citas** (puede estar vacío `[]`):

```json
[
  {
    "citCodigo": 1,
    "citTipoCita": "presencial",
    "citFechaProgramada": "2026-03-21",
    "citHoraInicio": "09:00:00",
    "citHoraFin": "10:00:00",
    "citMotivo": "Seguimiento de ansiedad",
    "citAnotaciones": "Notas de la sesión...",
    "citEstadoCita": "programada",
    "aprendizCita": {
      "aprFicCodigo": 123,
      "aprendiz": {
        "codigo": 50,
        "nroDocumento": "123456789",
        "nombres": { "primerNombre": "Juan", "segundoNombre": "Carlos" },
        "apellidos": { "primerApellido": "Pérez", "segundoApellido": "López" },
        "contacto": {
          "telefono": "3001234567",
          "correoInstitucional": "juan@sena.edu.co",
          "correoPersonal": "juan@gmail.com"
        }
      },
      "ficha": {
        "ficCodigo": 2589634,
        "ficJornada": "Diurna",
        "programaFormacion": {
          "progNombre": "ADSO",
          "area": { "areaNombre": "Desarrollo de Software" },
          "centro": { "cenNombre": "Centro X" }
        }
      }
    },
    "psicologo": {
      "psiCodigo": 1,
      "psiDocumento": "12345678",
      "psiNombre": "María",
      "psiApellido": "García"
    }
  }
]
```

### 401 Unauthorized
- Token ausente o inválido.
- Token expirado → usar `POST /api/Autenticacion/refrescar` con `refreshToken`.

### 403 Forbidden
- El usuario no tiene rol `Psicologo`.
- El JWT no contiene `NameIdentifier` (psiCodigo).

### 404 Not Found
- No aplica a este endpoint (siempre devuelve 200 con array, aunque vacío).

---

## 4. Pruebas en Swagger

1. Abre Swagger: `https://localhost:7xxx/swagger` (o tu URL local).
2. Autoriza:
   - Clic en **Authorize**.
   - En **Bearer**, pega tu `accessToken` (sin la palabra "Bearer").
   - Clic en **Authorize** y luego **Close**.
3. En `GET /api/Citas/agenda`:
   - Introduce `desde` y `hasta` (ej: `2026-03-16`, `2026-03-22`).
   - Clic en **Execute**.
4. Revisa el código de estado y el cuerpo de la respuesta.

---

## 5. Prueba rápida con cURL

```bash
# 1. Login (reemplaza correo y contraseña)
TOKEN=$(curl -s -X POST "http://localhost:5000/api/Autenticacion/ValidarPsicologo" \
  -H "Content-Type: application/json" \
  -d '{"correoPersonal":"tu_correo@ejemplo.com","password":"tu_password"}' \
  | jq -r '.accessToken // .token // .data.accessToken // empty')

# 2. Si obtuviste token, probar agenda
if [ -n "$TOKEN" ]; then
  curl -s -X GET "http://localhost:5000/api/Citas/agenda?desde=2026-03-01&hasta=2026-03-31" \
    -H "Authorization: Bearer $TOKEN" | jq .
else
  echo "No se obtuvo token. Verifica credenciales."
fi
```

*(Ajusta puerto y host según tu configuración.)*

---

## 6. Checklist antes de subir a producción

- [ ] Login como psicólogo devuelve `accessToken`.
- [ ] Request con `Authorization: Bearer {token}` devuelve 200.
- [ ] La respuesta es un array (aunque vacío).
- [ ] Cada cita tiene `citCodigo`, `citFechaProgramada`, `citHoraInicio`, `citHoraFin`, `aprendizCita`, `psicologo`.
- [ ] Sin token → 401.
- [ ] Con token de Aprendiz/Admin (no psicólogo) → 403.
- [ ] Parámetros `desde`/`hasta` opcionales funcionan.
- [ ] Rango > 90 días se recorta automáticamente.
