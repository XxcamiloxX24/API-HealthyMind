/**
 * Genera API-healthyMind.postman_collection.json en la raíz del repo (un nivel arriba de scripts/).
 * Ejecutar: node scripts/generate-postman-collection.mjs
 */
import { writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __dir = dirname(fileURLToPath(import.meta.url));
// Solo la colección en la raíz del repo (Servicios HealthyMind), no dentro de API-healthyMind
const repoRoot = join(__dir, "..", "..");
const outPath = join(repoRoot, "API-healthyMind.postman_collection.json");

const bearer = {
  type: "bearer",
  bearer: [{ key: "token", value: "{{accessToken}}", type: "string" }],
};

function req(method, path, name, opts = {}) {
  const { noAuth, query, bodyRaw, desc } = opts;
  const segments = path.split("/").filter(Boolean);
  const url = {
    raw: `{{baseUrl}}/${segments.join("/")}${query ? "?" + query : ""}`,
    host: ["{{baseUrl}}"],
    path: segments,
  };
  if (query) {
    url.query = query.split("&").map((p) => {
      const [key, value] = p.split("=");
      return { key, value: value ?? "" };
    });
  }
  const r = {
    name: name || `${method} ${path}`,
    request: {
      method,
      header: [{ key: "Content-Type", value: "application/json", type: "text", disabled: method === "GET" }],
      url,
    },
  };
  if (!noAuth) r.request.auth = { ...bearer };
  if (bodyRaw) {
    r.request.body = { mode: "raw", raw: bodyRaw };
    r.request.header[0].disabled = false;
  }
  if (desc) r.request.description = desc;
  return r;
}

const folders = [
  {
    name: "00 Autenticación (sin JWT salvo refrescar)",
    item: [
      req("GET", "/api/Autenticacion/debug-admin-config", "debug-admin-config (solo Development)", { noAuth: true }),
      req("POST", "/api/Autenticacion/ValidarAdmin", "ValidarAdmin", {
        noAuth: true,
        bodyRaw: '{\n  "correoPersonal": "",\n  "password": ""\n}',
      }),
      req("POST", "/api/Autenticacion/ValidarPsicologo", "ValidarPsicologo", {
        noAuth: true,
        bodyRaw: '{\n  "correoPersonal": "",\n  "password": ""\n}',
      }),
      req("POST", "/api/Autenticacion/ValidarAprendiz", "ValidarAprendiz", {
        noAuth: true,
        bodyRaw: '{\n  "correoPersonal": "",\n  "password": ""\n}',
      }),
      req("POST", "/api/Autenticacion/refrescar", "refrescar token", {
        noAuth: true,
        bodyRaw: '{\n  "refreshToken": ""\n}',
      }),
      req("GET", "/api/Autenticacion/debug-auth", "debug-auth", { noAuth: true }),
    ],
  },
  {
    name: "Aprendiz",
    item: [
      req("GET", "/api/Aprendiz", "Obtener todos"),
      req("GET", "/api/Aprendiz/listar", "listar", { query: "Pagina=1&TamanoPagina=10" }),
      req("GET", "/api/Aprendiz/50", "por id (PsiCodigo numérico aprendiz)"),
      req("GET", "/api/Aprendiz/busqueda-dinamica", "busqueda-dinamica", { query: "texto=abc" }),
      req("GET", "/api/Aprendiz/buscar", "buscar (query FiltroAprendizDTO)"),
      req("GET", "/api/Aprendiz/estadistica/crecimiento-mensual", "estadistica crecimiento-mensual"),
      req("GET", "/api/Aprendiz/estadistica/total-registrados", "estadistica total-registrados"),
      req("GET", "/api/Aprendiz/estadistica/por-mes", "estadistica por-mes"),
      req("POST", "/api/Aprendiz/registro-inicial", "registro-inicial", { noAuth: true, bodyRaw: "{}" }),
      req("POST", "/api/Aprendiz/verificar-codigo", "verificar-codigo", { noAuth: true, bodyRaw: "{}" }),
      req("POST", "/api/Aprendiz/reenviar-codigo", "reenviar-codigo", { noAuth: true, bodyRaw: "{}" }),
      req("PUT", "/api/Aprendiz/completar-informacion", "completar-informacion", { bodyRaw: "{}" }),
      req("PUT", "/api/Aprendiz/cambiar-correo", "cambiar-correo", { bodyRaw: "{}" }),
      req("PUT", "/api/Aprendiz/cambiar-password", "cambiar-password", { bodyRaw: "{}" }),
      req("POST", "/api/Aprendiz/recuperar-password", "recuperar-password", { noAuth: true, bodyRaw: "{}" }),
      req("POST", "/api/Aprendiz/reset-password", "reset-password", { noAuth: true, bodyRaw: "{}" }),
      req("POST", "/api/Aprendiz", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Aprendiz/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Aprendiz/cambiar-estado/doc", "cambiar-estado por documento", { bodyRaw: "{}" }),
      req("DELETE", "/api/Aprendiz/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "Area",
    item: [
      req("GET", "/api/Area", "Obtener todos"),
      req("GET", "/api/Area/1", "por id"),
      req("GET", "/api/Area/nombre/SISTEMAS", "por nombre"),
      req("POST", "/api/Area", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Area/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Area/eliminar/1", "eliminar (soft)"),
    ],
  },
  {
    name: "AprendizFicha",
    item: [
      req("GET", "/api/AprendizFicha", "Obtener todos"),
      req("GET", "/api/AprendizFicha/buscar", "buscar", { query: "FichaCodigo=1" }),
      req("GET", "/api/AprendizFicha/estadistica/por-ficha", "estadistica por-ficha"),
      req("POST", "/api/AprendizFicha", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/AprendizFicha/editar/doc", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/AprendizFicha/cambiar-estado", "cambiar-estado", { bodyRaw: '""' }),
      req("DELETE", "/api/AprendizFicha/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "CardsInfo",
    item: [
      req("GET", "/api/CardsInfo", "todos"),
      req("GET", "/api/CardsInfo/activos", "activos"),
      req("POST", "/api/CardsInfo", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/CardsInfo/editar/1", "editar", { bodyRaw: "{}" }),
      req("DELETE", "/api/CardsInfo/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "Centro",
    item: [
      req("GET", "/api/Centro", "todos"),
      req("GET", "/api/Centro/regional/1", "por regional"),
      req("GET", "/api/Centro/nodos", "nodos"),
      req("POST", "/api/Centro", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Centro/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Centro/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "CategoriaPregunta / CategoriaRespuesta",
    item: [
      req("GET", "/api/CategoriaPregunta", "CategoriaPregunta GET"),
      req("POST", "/api/CategoriaPregunta", "CategoriaPregunta POST", { bodyRaw: "{}" }),
      req("PUT", "/api/CategoriaPregunta/editar/1", "CategoriaPregunta editar", { bodyRaw: "{}" }),
      req("DELETE", "/api/CategoriaPregunta/eliminar/1", "CategoriaPregunta eliminar"),
      req("GET", "/api/CategoriaRespuesta", "CategoriaRespuesta GET"),
      req("POST", "/api/CategoriaRespuesta", "CategoriaRespuesta POST", { bodyRaw: "{}" }),
      req("PUT", "/api/CategoriaRespuesta/editar/1", "CategoriaRespuesta editar", { bodyRaw: "{}" }),
      req("DELETE", "/api/CategoriaRespuesta/eliminar/1", "CategoriaRespuesta eliminar"),
    ],
  },
  {
    name: "Ciudad",
    item: [
      req("GET", "/api/Ciudad", "todos"),
      req("GET", "/api/Ciudad/regional/1", "por regional"),
      req("GET", "/api/Ciudad/buscar", "buscar"),
    ],
  },
  {
    name: "Citas",
    item: [
      req("GET", "/api/Citas/agenda", "agenda (psicólogo, rango fechas)", {
        query: "desde=2026-03-16&hasta=2026-03-22",
        desc: "Citas del psicólogo autenticado. Requiere rol Psicólogo. Parámetros opcionales: desde, hasta (YYYY-MM-DD).",
      }),
      req("GET", "/api/Citas", "listar (filtros query)"),
      req("GET", "/api/Citas/listar-todas", "listar-todas"),
      req("GET", "/api/Citas/listar-activas", "listar-activas"),
      req("GET", "/api/Citas/buscar", "buscar"),
      req("GET", "/api/Citas/estadistica/comparacion-semanal", "comparacion-semanal"),
      req("GET", "/api/Citas/estadistica/por-mes", "por-mes"),
      req("GET", "/api/Citas/estadistica/actividad-exitosa", "actividad-exitosa"),
      req("GET", "/api/Citas/citas/estado-proceso", "estado-proceso"),
      req("GET", "/api/Citas/citas/estado-incidencias", "estado-incidencias"),
      req("GET", "/api/Citas/estadistica/por-estado", "por-estado"),
      req("GET", "/api/Citas/estadistica/por-dia", "por-dia"),
      req("POST", "/api/Citas", "crear cita", { bodyRaw: "{}" }),
      req("GET", "/api/Citas/test-email", "test-email"),
      req("GET", "/api/Citas/mis-citas", "mis-citas"),
      req("POST", "/api/Citas/solicitar-cita", "solicitar-cita", { bodyRaw: "{}" }),
      req("PUT", "/api/Citas/cancelar-mi-solicitud/1", "cancelar-mi-solicitud"),
      req("PUT", "/api/Citas/cancelar-cita/1", "cancelar-cita"),
      req("PUT", "/api/Citas/editar-solicitudes/1", "editar-solicitudes", { bodyRaw: "{}" }),
      req("PUT", "/api/Citas/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Citas/cambiar-estado-registro/1", "cambiar-estado-registro"),
    ],
  },
  {
    name: "Diario",
    item: [
      req("GET", "/api/Diario", "todos"),
      req("GET", "/api/Diario/activos", "activos"),
      req("GET", "/api/Diario/buscar", "buscar"),
      req("POST", "/api/Diario", "crear", {
        bodyRaw: '{\n  "diaTitulo": "Mi diario",\n  "diaImagenUrl": "https://ejemplo.com/imagen.jpg",\n  "diaAprendizFk": 1\n}',
      }),
      req("PUT", "/api/Diario/editar/1", "editar", {
        bodyRaw: '{\n  "diaTitulo": "Mi diario",\n  "diaImagenUrl": "https://ejemplo.com/imagen.jpg",\n  "diaAprendizFk": 1\n}',
      }),
      req("PUT", "/api/Diario/eliminar/1", "eliminar soft"),
    ],
  },
  {
    name: "PaginaDiario",
    item: [
      req("GET", "/api/PaginaDiario", "todas páginas"),
      req("GET", "/api/PaginaDiario/activos", "activas"),
      req("GET", "/api/PaginaDiario/diario/1", "páginas por diarioId (contenido + emoción + imagen)"),
      req("GET", "/api/PaginaDiario/paginacion-por-fecha", "por fecha (todas las entradas del día)", {
        query: "diarioId=1",
        desc: "Opcional: fecha=2025-03-14. Sin fecha = día más reciente con entradas.",
      }),
      req("POST", "/api/PaginaDiario", "crear página", {
        bodyRaw: '{\n  "pagTitulo": "Entrada del día",\n  "pagContenido": "Texto...",\n  "pagImagenUrl": "https://ejemplo.com/foto.jpg",\n  "pagDiarioFk": 1,\n  "pagEmocionFk": 1\n}',
      }),
      req("PUT", "/api/PaginaDiario/editar/1", "editar", {
        bodyRaw: '{\n  "pagTitulo": "Entrada del día",\n  "pagContenido": "Texto...",\n  "pagImagenUrl": "https://ejemplo.com/foto.jpg",\n  "pagDiarioFk": 1,\n  "pagEmocionFk": 1\n}',
      }),
      req("PUT", "/api/PaginaDiario/eliminar/1", "eliminar (nota: actúa sobre Diario por id)"),
    ],
  },
  {
    name: "Emociones",
    item: [
      req("GET", "/api/Emociones", "todos"),
      req("GET", "/api/Emociones/1", "por id"),
      req("POST", "/api/Emociones", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Emociones/editar/1", "editar", { bodyRaw: "{}" }),
      req("DELETE", "/api/Emociones/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "EstadoAprendiz / NivelFormacion / Regional",
    item: [
      req("GET", "/api/EstadoAprendiz", "EstadoAprendiz"),
      req("GET", "/api/NivelFormacion", "NivelFormacion"),
      req("GET", "/api/Regional", "Regional"),
    ],
  },
  {
    name: "Ficha",
    item: [
      req("GET", "/api/Ficha", "todos"),
      req("GET", "/api/Ficha/mis-fichas", "mis-fichas (solo Psicologo)", {
        desc: "Requiere JWT rol Psicologo",
      }),
      req("GET", "/api/Ficha/1", "por id"),
      req("GET", "/api/Ficha/listar", "listar paginado", { query: "Pagina=1&TamanoPagina=10" }),
      req("GET", "/api/Ficha/busqueda-dinamica", "busqueda-dinamica", { query: "texto=abc" }),
      req("GET", "/api/Ficha/buscar", "buscar filtros", { query: "AreaNombre=SISTEMAS" }),
      req("POST", "/api/Ficha", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Ficha/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Ficha/cambiar-estado/1", "cambiar-estado"),
      req("DELETE", "/api/Ficha/eliminar/1", "eliminar definitivo"),
    ],
  },
  {
    name: "ProgramaFormacion",
    item: [
      req("GET", "/api/ProgramaFormacion", "todos"),
      req("GET", "/api/ProgramaFormacion/1", "por id"),
      req("GET", "/api/ProgramaFormacion/buscar", "buscar"),
      req("POST", "/api/ProgramaFormacion", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/ProgramaFormacion/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/ProgramaFormacion/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "Psicologo",
    item: [
      req("GET", "/api/Psicologo", "todos"),
      req("GET", "/api/Psicologo/listar", "listar", { query: "Pagina=1&TamanoPagina=10" }),
      req("GET", "/api/Psicologo/1", "por id (PsiCodigo)"),
      req("GET", "/api/Psicologo/busqueda-dinamica", "busqueda-dinamica", { query: "texto=ab" }),
      req("GET", "/api/Psicologo/area/SISTEMAS", "por area nombre"),
      req("GET", "/api/Psicologo/estadistica/total-activos", "total-activos"),
      req("POST", "/api/Psicologo", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/Psicologo/editar/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Psicologo/cambiar-password", "cambiar-password", { bodyRaw: "{}" }),
      req("POST", "/api/Psicologo/recuperar-password", "recuperar-password", { noAuth: true, bodyRaw: "{}" }),
      req("POST", "/api/Psicologo/reset-password", "reset-password", { noAuth: true, bodyRaw: "{}" }),
      req("PUT", "/api/Psicologo/cambiar-estado/1", "cambiar-estado"),
      req("DELETE", "/api/Psicologo/eliminar/1", "eliminar"),
    ],
  },
  {
    name: "Preguntas / Respuestas",
    item: [
      req("GET", "/api/Preguntas", "Preguntas"),
      req("POST", "/api/Preguntas", "Preguntas POST", { bodyRaw: "{}" }),
      req("PUT", "/api/Preguntas/editar/1", "Preguntas editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Preguntas/eliminar/1", "Preguntas eliminar"),
      req("GET", "/api/Respuestas", "Respuestas"),
      req("POST", "/api/Respuestas", "Respuestas POST", { bodyRaw: "{}" }),
      req("PUT", "/api/Respuestas/editar/1", "Respuestas editar", { bodyRaw: "{}" }),
      req("PUT", "/api/Respuestas/eliminar/1", "Respuestas eliminar"),
    ],
  },
  {
    name: "SeguimientoAprendiz",
    item: [
      req("GET", "/api/SeguimientoAprendiz/estados", "estados catálogo"),
      req("GET", "/api/SeguimientoAprendiz", "todos"),
      req("GET", "/api/SeguimientoAprendiz/listar", "listar", { query: "Pagina=1&TamanoPagina=10" }),
      req("GET", "/api/SeguimientoAprendiz/mis-seguimientos", "mis-seguimientos (Psicologo)", { query: "Pagina=1&TamanoPagina=10" }),
      req("GET", "/api/SeguimientoAprendiz/1", "por id"),
      req("GET", "/api/SeguimientoAprendiz/buscar", "buscar"),
      req("GET", "/api/SeguimientoAprendiz/estadistica/por-mes-inicio", "por-mes-inicio"),
      req("GET", "/api/SeguimientoAprendiz/estadistica/tendencia-estado", "tendencia-estado"),
      req("GET", "/api/SeguimientoAprendiz/estadistica/por-mes-fin", "por-mes-fin"),
      req("GET", "/api/SeguimientoAprendiz/estadistica/por-dia-inicio", "por-dia-inicio"),
      req("GET", "/api/SeguimientoAprendiz/estadistica/por-dia-fin", "por-dia-fin"),
      req("POST", "/api/SeguimientoAprendiz", "crear", { bodyRaw: "{}" }),
      req("PUT", "/api/SeguimientoAprendiz/1", "editar", { bodyRaw: "{}" }),
      req("PUT", "/api/SeguimientoAprendiz/eliminar/1", "eliminar soft"),
    ],
  },
  {
    name: "TestGeneral / TestPreguntas",
    item: [
      req("GET", "/api/TestGeneral", "TestGeneral todos"),
      req("GET", "/api/TestGeneral/1", "TestGeneral por id"),
      req("GET", "/api/TestGeneral/buscar", "TestGeneral buscar"),
      req("POST", "/api/TestGeneral", "TestGeneral crear", { bodyRaw: "{}" }),
      req("PUT", "/api/TestGeneral/editar/1", "TestGeneral editar", { bodyRaw: "{}" }),
      req("PUT", "/api/TestGeneral/eliminar/1", "TestGeneral eliminar"),
      req("GET", "/api/TestPreguntas", "TestPreguntas todos"),
      req("GET", "/api/TestPreguntas/buscar", "TestPreguntas buscar"),
      req("POST", "/api/TestPreguntas", "TestPreguntas crear", { bodyRaw: "{}" }),
      req("PUT", "/api/TestPreguntas/editar/1", "TestPreguntas editar", { bodyRaw: "{}" }),
      req("PUT", "/api/TestPreguntas/eliminar/1", "TestPreguntas eliminar"),
    ],
  },
];

const collection = {
  info: {
    name: "API-healthyMind",
    description:
      "Colección generada desde los controladores actuales. Variables: baseUrl (ej. https://localhost:XXXX), accessToken (Bearer del login). Muchas rutas requieren JWT según rol.",
    schema: "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
  },
  variable: [
    { key: "baseUrl", value: "https://localhost:7001" },
    { key: "accessToken", value: "" },
  ],
  auth: bearer,
  item: folders,
};

writeFileSync(outPath, JSON.stringify(collection, null, 2), "utf8");
console.log("Written:", outPath);
