-- ============================================================
-- Script: agregar campos de URL de imagen a Diario y PaginaDiario
-- Ejecutar en la base de datos de HealthyMind después de respaldo.
-- ============================================================

-- 1. Tabla diario: columna para el enlace de imagen del diario
ALTER TABLE diario
ADD COLUMN dia_imagen_url VARCHAR(500) NULL
COMMENT 'URL del enlace a la imagen asociada al diario'
AFTER dia_titulo;

-- 2. Tabla pagina_diario: columna para el enlace de imagen de la página
ALTER TABLE pagina_diario
ADD COLUMN pag_imagen_url VARCHAR(500) NULL
COMMENT 'URL del enlace a la imagen asociada a la página'
AFTER pag_contenido;

-- Comprobar que las columnas existen (opcional)
-- SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
-- FROM INFORMATION_SCHEMA.COLUMNS
-- WHERE TABLE_SCHEMA = DATABASE()
--   AND TABLE_NAME IN ('diario', 'pagina_diario')
--   AND COLUMN_NAME IN ('dia_imagen_url', 'pag_imagen_url');
