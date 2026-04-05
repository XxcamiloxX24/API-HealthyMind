-- ============================================================
-- MIGRACIÓN: Sistema de Tests/Evaluaciones (Plantillas)
-- Ejecutar en la base de datos MySQL de HealthyMind
-- ============================================================

-- 1. Tabla plantilla_test
CREATE TABLE IF NOT EXISTS `plantilla_test` (
    `pla_tst_codigo` INT NOT NULL AUTO_INCREMENT,
    `pla_tst_nombre` VARCHAR(255) NOT NULL,
    `pla_tst_descripcion` TEXT NULL,
    `pla_tst_psicologo_fk` INT NULL,
    `pla_tst_estado_registro` VARCHAR(20) NOT NULL DEFAULT 'activo',
    `pla_tst_fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`pla_tst_codigo`),
    INDEX `pla_tst_psicologoFK` (`pla_tst_psicologo_fk`),
    CONSTRAINT `plantilla_test_ibfk_1` FOREIGN KEY (`pla_tst_psicologo_fk`)
        REFERENCES `psicologo` (`psi_codigo`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Tabla plantilla_pregunta
CREATE TABLE IF NOT EXISTS `plantilla_pregunta` (
    `pla_prg_codigo` INT NOT NULL AUTO_INCREMENT,
    `pla_prg_plantilla_fk` INT NOT NULL,
    `pla_prg_texto` TEXT NOT NULL,
    `pla_prg_tipo` VARCHAR(30) NOT NULL DEFAULT 'opcion_multiple',
    `pla_prg_orden` INT NOT NULL DEFAULT 0,
    `pla_prg_estado_registro` VARCHAR(20) NOT NULL DEFAULT 'activo',
    PRIMARY KEY (`pla_prg_codigo`),
    INDEX `pla_prg_plantillaFK` (`pla_prg_plantilla_fk`),
    CONSTRAINT `plantilla_pregunta_ibfk_1` FOREIGN KEY (`pla_prg_plantilla_fk`)
        REFERENCES `plantilla_test` (`pla_tst_codigo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Tabla plantilla_opcion
CREATE TABLE IF NOT EXISTS `plantilla_opcion` (
    `pla_opc_codigo` INT NOT NULL AUTO_INCREMENT,
    `pla_opc_pregunta_fk` INT NOT NULL,
    `pla_opc_texto` VARCHAR(500) NOT NULL,
    `pla_opc_orden` INT NOT NULL DEFAULT 0,
    `pla_opc_estado_registro` VARCHAR(20) NOT NULL DEFAULT 'activo',
    PRIMARY KEY (`pla_opc_codigo`),
    INDEX `pla_opc_preguntaFK` (`pla_opc_pregunta_fk`),
    CONSTRAINT `plantilla_opcion_ibfk_1` FOREIGN KEY (`pla_opc_pregunta_fk`)
        REFERENCES `plantilla_pregunta` (`pla_prg_codigo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Modificar test_general: agregar columnas
ALTER TABLE `test_general`
    ADD COLUMN `test_gen_plantilla_fk` INT NULL AFTER `test_gen_recomendacion`,
    ADD COLUMN `test_gen_estado_test` VARCHAR(30) NOT NULL DEFAULT 'asignado' AFTER `test_gen_plantilla_fk`,
    ADD INDEX `test_gen_plantillaFK` (`test_gen_plantilla_fk`),
    ADD CONSTRAINT `test_general_ibfk_3` FOREIGN KEY (`test_gen_plantilla_fk`)
        REFERENCES `plantilla_test` (`pla_tst_codigo`) ON DELETE SET NULL;

-- 5. Tabla test_respuesta
CREATE TABLE IF NOT EXISTS `test_respuesta` (
    `tes_res_codigo` INT NOT NULL AUTO_INCREMENT,
    `tes_res_test_fk` INT NOT NULL,
    `tes_res_pregunta_fk` INT NOT NULL,
    `tes_res_opcion_fk` INT NOT NULL,
    `tes_res_fecha_respuesta` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `tes_res_estado_registro` VARCHAR(20) NOT NULL DEFAULT 'activo',
    PRIMARY KEY (`tes_res_codigo`),
    INDEX `tes_res_testFK` (`tes_res_test_fk`),
    INDEX `tes_res_preguntaFK` (`tes_res_pregunta_fk`),
    INDEX `tes_res_opcionFK` (`tes_res_opcion_fk`),
    CONSTRAINT `test_respuesta_ibfk_1` FOREIGN KEY (`tes_res_test_fk`)
        REFERENCES `test_general` (`test_gen_codigo`) ON DELETE CASCADE,
    CONSTRAINT `test_respuesta_ibfk_2` FOREIGN KEY (`tes_res_pregunta_fk`)
        REFERENCES `plantilla_pregunta` (`pla_prg_codigo`) ON DELETE CASCADE,
    CONSTRAINT `test_respuesta_ibfk_3` FOREIGN KEY (`tes_res_opcion_fk`)
        REFERENCES `plantilla_opcion` (`pla_opc_codigo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
