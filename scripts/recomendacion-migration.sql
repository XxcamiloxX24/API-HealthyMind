-- ============================================================
-- Script: Migración Recomendaciones
-- Descripción:
--   1. Crea la tabla recomendacion para recomendaciones por seguimiento
--   2. Elimina el campo seg_recomendaciones de seguimiento_aprendiz
--
-- IMPORTANTE: Si necesitas conservar el contenido de seg_recomendaciones,
-- haz un respaldo antes:
--   CREATE TABLE seguimiento_aprendiz_recomendaciones_backup AS
--   SELECT seg_codigo, seg_recomendaciones FROM seguimiento_aprendiz
--   WHERE seg_recomendaciones IS NOT NULL AND seg_recomendaciones != '';
--
-- Ejecutar en el orden indicado (PASO 1 primero, luego PASO 2)
-- ============================================================

-- ------------------------------------------------------------
-- PASO 1: Crear tabla recomendacion (PRIMERO, antes de modificar seguimiento)
-- ------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `recomendacion` (
    `rec_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador de la recomendación',
    `rec_seguimiento_fk` int(40) NOT NULL COMMENT 'Seguimiento al que pertenece',
    `rec_titulo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Título de la recomendación',
    `rec_descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Descripción detallada',
    `rec_fecha_vencimiento` datetime NULL COMMENT 'Fecha de finalización: se establece al marcar como Completada',
    `rec_estado` enum('Pendiente','En Progreso','Completada') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'Pendiente' COMMENT 'Estado de la recomendación',
    `rec_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo' COMMENT 'Soft delete',
    `rec_fecha_creacion` datetime NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha de creación',
    `rec_fecha_actualizacion` datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Última actualización',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`rec_codigo`),
    CONSTRAINT `recomendacion_ibfk_1` FOREIGN KEY (`rec_seguimiento_fk`) REFERENCES `seguimiento_aprendiz` (`seg_codigo`) ON DELETE CASCADE,
    INDEX `rec_estado_registro` (`rec_estado_registro`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci
COMMENT='Recomendaciones asignadas por el psicólogo a cada seguimiento';


-- ------------------------------------------------------------
-- PASO 2: Eliminar columna seg_recomendaciones de seguimiento_aprendiz
-- ------------------------------------------------------------

ALTER TABLE `seguimiento_aprendiz`
    DROP COLUMN `seg_recomendaciones`;
