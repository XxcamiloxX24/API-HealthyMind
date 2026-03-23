-- Migración manual: ampliar columnas de firma para soportar URLs completas de Cloudinary
-- Las URLs típicas superan 100 caracteres y se truncaban, causando 404.
-- Ejecutar en la base de datos MySQL/MariaDB:

ALTER TABLE seguimiento_aprendiz
  MODIFY COLUMN seg_firma_profesional VARCHAR(500) NULL,
  MODIFY COLUMN seg_firma_aprendiz VARCHAR(500) NULL;
