using System;
using System.Collections.Generic;
using API_healthyMind.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace API_healthyMind.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aprendiz> Aprendizs { get; set; }
    public virtual DbSet<VerificationCode> VerificationCodes { get; set; }

    public virtual DbSet<AprendizFicha> AprendizFichas { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<CategoriaPreguntas> CategoriaPregunta { get; set; }

    public virtual DbSet<CategoriaRespuestas> CategoriaRespuesta { get; set; }

    public virtual DbSet<Centro> Centros { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Ciudad> Ciudads { get; set; }

    public virtual DbSet<Diario> Diarios { get; set; }

    public virtual DbSet<Emociones> Emociones { get; set; }

    public virtual DbSet<EstadoAprendiz> EstadoAprendizs { get; set; }

    public virtual DbSet<Ficha> Fichas { get; set; }

    public virtual DbSet<NivelFormacion> NivelFormacions { get; set; }

    public virtual DbSet<PaginaDiario> PaginaDiarios { get; set; }

    public virtual DbSet<Preguntas> Pregunta { get; set; }

    public virtual DbSet<Programaformacion> Programaformacions { get; set; }

    public virtual DbSet<Psicologo> Psicologos { get; set; }

    public virtual DbSet<Regional> Regionals { get; set; }

    public virtual DbSet<Respuestas> Respuesta { get; set; }

    public virtual DbSet<SeguimientoAprendiz> SeguimientoAprendizs { get; set; }

    public virtual DbSet<TestGeneral> TestGenerals { get; set; }

    public virtual DbSet<TestPreguntas> TestPregunta { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aprendiz>(entity =>
        {
            entity.HasKey(e => e.AprCodigo).HasName("PRIMARY");

            entity.ToTable("aprendiz");

            entity.HasIndex(e => e.AprCiudadFk, "apr_ciudadFK");

            entity.HasIndex(e => e.AprEstadoAprFk, "apr_estado_aprFK");

            entity.Property(e => e.AprCodigo)
                .HasComment("Codigo unico del aprendiz")
                .HasColumnType("int(40)")
                .HasColumnName("apr_codigo");
            entity.Property(e => e.AprFechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("apr_fechacreacion");
            entity.Property(e => e.AprAcudApellido)
                .HasMaxLength(100)
                .HasComment("apelldio del acudiente")
                .HasColumnName("apr_acud_apellido");
            entity.Property(e => e.AprAcudNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del acudiente")
                .HasColumnName("apr_acud_nombre");
            entity.Property(e => e.AprApellido)
                .HasMaxLength(100)
                .HasComment("Apellido del aprendiz")
                .HasColumnName("apr_apellido");
            entity.Property(e => e.AprCiudadFk)
                .HasColumnType("int(40)")
                .HasColumnName("apr_ciudadFK");
            entity.Property(e => e.AprCorreoInstitucional)
                .HasMaxLength(100)
                .HasColumnName("apr_correo_institucional");
            entity.Property(e => e.AprCorreoPersonal)
                .HasMaxLength(100)
                .HasComment("Correo del aprendiz")
                .HasColumnName("apr_correo_personal");
            entity.Property(e => e.AprDireccion)
                .HasMaxLength(100)
                .HasColumnName("apr_direccion");
            entity.Property(e => e.AprEps)
                .HasMaxLength(100)
                .HasColumnName("apr_eps");
            entity.Property(e => e.AprEstadoAprFk)
                .HasColumnType("int(40)")
                .HasColumnName("apr_estado_aprFK");
            entity.Property(e => e.AprEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasComment("Estado del registro")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("apr_estado_registro");
            entity.Property(e => e.AprFechaEliminacion)
                .HasColumnType("datetime")
                .HasColumnName("apr_fecha_eliminacion");
            entity.Property(e => e.AprFechaNac).HasColumnName("apr_fechaNac");
            entity.Property(e => e.AprFirma)
                .HasColumnType("blob")
                .HasColumnName("apr_firma");
            entity.Property(e => e.AprNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del aprendiz")
                .HasColumnName("apr_nombre");
            entity.Property(e => e.AprNroDocumento)
                .HasComment("Numero de documento")
                .HasColumnType("int(11)")
                .HasColumnName("apr_nro_documento");
            entity.Property(e => e.AprPassword)
                .HasMaxLength(100)
                .HasColumnName("apr_password");
            entity.Property(e => e.AprPatologia)
                .HasMaxLength(100)
                .HasColumnName("apr_patologia");
            entity.Property(e => e.AprRazonEliminacion)
                .HasMaxLength(200)
                .HasColumnName("apr_razon_eliminacion");
            entity.Property(e => e.AprSegundoApellido)
                .HasMaxLength(100)
                .HasComment("Segundo Apellido")
                .HasColumnName("apr_segundo_apellido");
            entity.Property(e => e.AprSegundoNombre)
                .HasMaxLength(100)
                .HasComment("Segundo nombre del aprendiz")
                .HasColumnName("apr_segundo_nombre");
            entity.Property(e => e.AprTelefono)
                .HasComment("Numero de celular")
                .HasColumnType("int(11)")
                .HasColumnName("apr_telefono");
            entity.Property(e => e.AprTelefonoAcudiente)
                .HasComment("Numero de celular de un acudiente")
                .HasColumnType("int(11)")
                .HasColumnName("apr_telefono_acudiente");
            entity.Property(e => e.AprTipoDocumento)
                .HasComment("Tipo de documento de identidad")
                .HasColumnType("enum('CC','TI','CE')")
                .HasColumnName("apr_tipo_documento");
            entity.Property(e => e.AprTipoPoblacion)
                .HasColumnType("enum('Desplazado','Negro','Discapacitado','Campesino','Afro','Gitano','Indigena','Ninguno')")
                .HasColumnName("apr_tipo_poblacion");

            entity.HasOne(d => d.Municipio).WithMany(p => p.Aprendizs)
                .HasForeignKey(d => d.AprCiudadFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("aprendiz_ibfk_2");

            entity.HasOne(d => d.EstadoAprendiz).WithMany(p => p.Aprendizs)
                .HasForeignKey(d => d.AprEstadoAprFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("aprendiz_ibfk_1");
        });

        modelBuilder.Entity<VerificationCode>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("verificationcode");

            entity.HasIndex(e => e.AprendizId, "AprendizId");


            entity.Property(e => e.id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AprendizId)
                .HasColumnType("int(11)")
                .HasColumnName("AprendizId");
            entity.Property(e => e.Codigo)
                .HasMaxLength(6)
                .HasColumnName("Codigo");
            entity.Property(e => e.Expiration)
                .HasColumnType("datetime")
                .HasColumnName("Expiration");
            
            

            entity.HasOne(d => d.Aprendiz).WithMany(p => p.VerificationCode)
                .HasForeignKey(d => d.AprendizId)
                .HasConstraintName("verificationcode_ibfk_1");
        });

        modelBuilder.Entity<AprendizFicha>(entity =>
        {
            entity.HasKey(e => e.AprFicCodigo).HasName("PRIMARY");

            entity.ToTable("aprendiz_ficha");

            entity.HasIndex(e => e.AprFicAprendizFk, "apr_fic_aprendizFK");

            entity.HasIndex(e => e.AprFicCodigo, "apr_fic_codigo");

            entity.HasIndex(e => e.AprFicFichaFk, "apr_fic_fichaFK");

            entity.Property(e => e.AprFicCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("apr_fic_codigo");
            entity.Property(e => e.AprFicAprendizFk)
                .HasColumnType("int(40)")
                .HasColumnName("apr_fic_aprendizFK");
            entity.Property(e => e.AprFicEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("apr_fic_estado_registro");
            entity.Property(e => e.AprFicFichaFk)
                .HasColumnType("int(40)")
                .HasColumnName("apr_fic_fichaFK");

            entity.HasOne(d => d.Aprendiz).WithMany(p => p.AprendizFichas)
                .HasForeignKey(d => d.AprFicAprendizFk)
                .HasConstraintName("aprendiz_ficha_ibfk_2");

            entity.HasOne(d => d.Ficha).WithMany(p => p.AprendizFichas)
                .HasForeignKey(d => d.AprFicFichaFk)
                .HasConstraintName("aprendiz_ficha_ibfk_1");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaCodigo).HasName("PRIMARY");

            entity.ToTable("area");

            entity.HasIndex(e => e.AreaPsicCodFk, "area_psic_codFK");


            entity.Property(e => e.AreaCodigo)
                .HasComment("Identificador de la facultad o area")
                .HasColumnType("int(40)")
                .HasColumnName("area_codigo");
            entity.Property(e => e.AreaPsicCodFk)
                .HasColumnType("int(40)")
                .HasColumnName("area_psic_codFK");

            entity.HasOne(d => d.AreaPsicologo).WithMany(d => d.Area)
                .HasForeignKey(d => d.AreaPsicCodFk)
                .HasConstraintName("area_ibfk_1");

            entity.Property(e => e.AreaEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("area_estado_registro");
            entity.Property(e => e.AreaNombre)
                .HasMaxLength(100)
                .HasComment("Nombre de la facultad o area")
                .HasColumnName("area_nombre");
        });

        modelBuilder.Entity<CategoriaPreguntas>(entity =>
        {
            entity.HasKey(e => e.CatPregCodigo).HasName("PRIMARY");

            entity.ToTable("categoria_pregunta");

            entity.Property(e => e.CatPregCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("cat_preg_codigo");
            entity.Property(e => e.CatPregDescripcion)
                .HasColumnType("text")
                .HasColumnName("cat_preg_descripcion");
            entity.Property(e => e.CatPregNombre)
                .HasMaxLength(100)
                .HasColumnName("cat_preg_nombre");
        });

        modelBuilder.Entity<CategoriaRespuestas>(entity =>
        {
            entity.HasKey(e => e.CatResCodigo).HasName("PRIMARY");

            entity.ToTable("categoria_respuesta");

            entity.Property(e => e.CatResCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("cat_res_codigo");
            entity.Property(e => e.CatResDescripcion)
                .HasColumnType("text")
                .HasColumnName("cat_res_descripcion");
            entity.Property(e => e.CatResNombre)
                .HasMaxLength(100)
                .HasColumnName("cat_res_nombre");
        });

        modelBuilder.Entity<Centro>(entity =>
        {
            entity.HasKey(e => e.CenCodigo).HasName("PRIMARY");

            entity.ToTable("centro");

            entity.HasIndex(e => e.CenCodFk, "cen_codFK");

            entity.HasIndex(e => e.CenRegCodFk, "cen_reg_codFK");

            entity.Property(e => e.CenCodigo)
                .HasComment("identificador del centro")
                .HasColumnType("int(40)")
                .HasColumnName("cen_codigo");
            entity.Property(e => e.CenCodFk)
                .HasColumnType("int(40)")
                .HasColumnName("cen_codFK");
            entity.Property(e => e.CenDireccion)
                .HasMaxLength(100)
                .HasComment("direccion del centro")
                .HasColumnName("cen_direccion");
            entity.Property(e => e.CenEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("cen_estado_registro");
            entity.Property(e => e.CenNombre)
                .HasMaxLength(100)
                .HasComment("nombre del centro")
                .HasColumnName("cen_nombre");
            entity.Property(e => e.CenRegCodFk)
                .HasColumnType("int(40)")
                .HasColumnName("cen_reg_codFK");

            entity.HasOne(d => d.centroPadre).WithMany(p => p.InverseCenCodFkNavigation)
                .HasForeignKey(d => d.CenCodFk)
                .HasConstraintName("centro_ibfk_2");

            entity.HasOne(d => d.Regional).WithMany(p => p.Centros)
                .HasForeignKey(d => d.CenRegCodFk)
                .HasConstraintName("centro_ibfk_1");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.CitCodigo).HasName("PRIMARY");

            entity.ToTable("citas");

            entity.HasIndex(e => e.CitAprCodFk, "cit_apr_codFK");

            entity.HasIndex(e => e.CitPsiCodFk, "cit_psi_codFK");

            entity.Property(e => e.CitCodigo)
                .HasComment("Identificador de la cita")
                .HasColumnType("int(40)")
                .HasColumnName("cit_codigo");
            entity.Property(e => e.CitAnotaciones)
                .HasComment("Anotaciones u observaciones antes o despues de la atencio")
                .HasColumnType("text")
                .HasColumnName("cit_anotaciones");
            entity.Property(e => e.CitAprCodFk)
                .HasComment("Aprendiz")
                .HasColumnType("int(40)")
                .HasColumnName("cit_apr_codFK");
            entity.Property(e => e.CitEstadoCita)
                .HasComment("estado de la cita")
                .HasColumnType("enum('programada','realizada','cancelada','reprogramada','no asistió')")
                .HasColumnName("cit_estado_cita");
            entity.Property(e => e.CitEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("cit_estado_registro");
            entity.Property(e => e.CitFechaCreacion)
                .HasComment("fecha en la que se realizo el registro de la cita")
                .HasColumnType("datetime")
                .HasColumnName("cit_fecha_creacion");
            entity.Property(e => e.CitFechaProgramada)
                .HasComment("Fecha en la que fue programada la cita")
                .HasColumnName("cit_fecha_programada");
            entity.Property(e => e.CitHoraFin)
                .HasComment("Hora de fin de la cita")
                .HasColumnType("time")
                .HasColumnName("cit_hora_fin");
            entity.Property(e => e.CitHoraInicio)
                .HasComment("Hora de inicio de la cita")
                .HasColumnType("time")
                .HasColumnName("cit_hora_inicio");
            entity.Property(e => e.CitMotivo)
                .HasComment("Motivo de la cita")
                .HasColumnType("text")
                .HasColumnName("cit_motivo");
            entity.Property(e => e.CitMotivoSolicitud)
                .HasMaxLength(200)
                .HasColumnName("cit_motivo_solicitud");
            entity.Property(e => e.CitPsiCodFk)
                .HasComment("Psicologo")
                .HasColumnType("int(40)")
                .HasColumnName("cit_psi_codFK");
            entity.Property(e => e.CitTipoCita)
                .HasComment("Tipo de cita")
                .HasColumnType("enum('videollamada','chat','presencial')")
                .HasColumnName("cit_tipo_cita");

            entity.HasOne(d => d.CitAprCodFkNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.CitAprCodFk)
                .HasConstraintName("citas_ibfk_3");

            entity.HasOne(d => d.CitPsiCodFkNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.CitPsiCodFk)
                .HasConstraintName("citas_ibfk_2");
        });

        modelBuilder.Entity<Ciudad>(entity =>
        {
            entity.HasKey(e => e.CiuCodigo).HasName("PRIMARY");

            entity.ToTable("ciudad");

            entity.HasIndex(e => e.CiuRegionalFk, "ciu_regionalFK");

            entity.Property(e => e.CiuCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("ciu_codigo");
            entity.Property(e => e.CiuNombre)
                .HasMaxLength(100)
                .HasColumnName("ciu_nombre");
            entity.Property(e => e.CiuRegionalFk)
                .HasColumnType("int(40)")
                .HasColumnName("ciu_regionalFK");

            entity.HasOne(d => d.Regional).WithMany(p => p.Ciudads)
                .HasForeignKey(d => d.CiuRegionalFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ciudad_ibfk_1");
        });

        modelBuilder.Entity<Diario>(entity =>
        {
            entity.HasKey(e => e.DiaCodigo).HasName("PRIMARY");

            entity.ToTable("diario");

            entity.HasIndex(e => e.DiaAprendizFk, "dia_aprendizFK");

            entity.Property(e => e.DiaCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("dia_codigo");
            entity.Property(e => e.DiaAprendizFk)
                .HasColumnType("int(40)")
                .HasColumnName("dia_aprendizFK");
            entity.Property(e => e.DiaEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("dia_estado_registro");
            entity.Property(e => e.DiaFechaCreacion).HasColumnName("dia_fecha_creacion");
            entity.Property(e => e.DiaTitulo)
                .HasMaxLength(100)
                .HasColumnName("dia_titulo");

            entity.HasOne(d => d.DiaAprendizFkNavigation).WithMany(p => p.Diarios)
                .HasForeignKey(d => d.DiaAprendizFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("diario_ibfk_1");
        });

        modelBuilder.Entity<Emociones>(entity =>
        {
            entity.HasKey(e => e.EmoCodigo).HasName("PRIMARY");

            entity.ToTable("emociones");

            entity.Property(e => e.EmoCodigo)
                .HasComment("identificador de la emocion")
                .HasColumnType("int(40)")
                .HasColumnName("emo_codigo");
            entity.Property(e => e.EmoDescripcion)
                .HasComment("descripcion de la emocion")
                .HasColumnType("text")
                .HasColumnName("emo_descripcion");
            entity.Property(e => e.EmoImage)
                .HasMaxLength(100)
                .HasComment("url de la imagen de la emocion")
                .HasColumnName("emo_image");
            entity.Property(e => e.EmoNombre)
                .HasMaxLength(100)
                .HasComment("nombre de la emocion")
                .HasColumnName("emo_nombre");
        });

        modelBuilder.Entity<EstadoAprendiz>(entity =>
        {
            entity.HasKey(e => e.EstAprCodigo).HasName("PRIMARY");

            entity.ToTable("estado_aprendiz");

            entity.Property(e => e.EstAprCodigo)
                .HasComment("Identificador del estado")
                .HasColumnType("int(40)")
                .HasColumnName("est_apr_codigo");
            entity.Property(e => e.EstAprDescrip)
                .HasComment("Descripcion del estado")
                .HasColumnType("text")
                .HasColumnName("est_apr_descrip");
            entity.Property(e => e.EstAprNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del estado")
                .HasColumnName("est_apr_nombre");
        });

        modelBuilder.Entity<Ficha>(entity =>
        {
            entity.HasKey(e => e.FicCodigo).HasName("PRIMARY");

            entity.ToTable("ficha");

            entity.HasIndex(e => e.FicProgramaFk, "fic_programaFK");

            entity.Property(e => e.FicCodigo)
                .ValueGeneratedNever()
                .HasComment("Numero de ficha")
                .HasColumnType("int(40)")
                .HasColumnName("fic_codigo");
            entity.Property(e => e.FicEstadoFormacion)
                .HasComment("Estado de formacion de la ficha")
                .HasColumnType("enum('en ejecucion','cancelada','terminada por fecha','terminada')")
                .HasColumnName("fic_estado_formacion");
            entity.Property(e => e.FicEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("fic_estado_registro");
            entity.Property(e => e.FicFechaFin).HasColumnName("fic_fecha_fin");
            entity.Property(e => e.FicFechaInicio).HasColumnName("fic_fecha_inicio");
            entity.Property(e => e.FicJornada)
                .HasComment("Tipo de jornada de la ficha")
                .HasColumnType("enum('diurna','nocturna','madrugada','mixta')")
                .HasColumnName("fic_jornada");
            entity.Property(e => e.FicProgramaFk)
                .HasColumnType("int(40)")
                .HasColumnName("fic_programaFK");

            entity.HasOne(d => d.programaFormacion).WithMany(p => p.Fichas)
                .HasForeignKey(d => d.FicProgramaFk)
                .HasConstraintName("ficha_ibfk_1");
        });

        modelBuilder.Entity<NivelFormacion>(entity =>
        {
            entity.HasKey(e => e.NivForCodigo).HasName("PRIMARY");

            entity.ToTable("nivel_formacion");

            entity.Property(e => e.NivForCodigo)
                .HasComment("Identificador del tipo de formacion")
                .HasColumnType("int(40)")
                .HasColumnName("niv_for_codigo");
            entity.Property(e => e.NivForDescripcion)
                .HasComment("Descripcion del tipo de formacion")
                .HasColumnType("text")
                .HasColumnName("niv_for_descripcion");
            entity.Property(e => e.NivForNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del tipo de formacion")
                .HasColumnName("niv_for_nombre");
        });

        modelBuilder.Entity<PaginaDiario>(entity =>
        {
            entity.HasKey(e => e.PagCodigo).HasName("PRIMARY");

            entity.ToTable("pagina_diario");

            entity.HasIndex(e => e.PagDiarioFk, "pag_diarioFK");

            entity.HasIndex(e => e.PagEmocionFk, "pag_emocionFK");

            entity.Property(e => e.PagCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("pag_codigo");
            entity.Property(e => e.PagContenido)
                .HasColumnType("text")
                .HasColumnName("pag_contenido");
            entity.Property(e => e.PagDiarioFk)
                .HasColumnType("int(40)")
                .HasColumnName("pag_diarioFK");
            entity.Property(e => e.PagEmocionFk)
                .HasColumnType("int(40)")
                .HasColumnName("pag_emocionFK");
            entity.Property(e => e.PagEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("pag_estado_registro");
            entity.Property(e => e.PagFechaRealizacion)
                .HasColumnType("datetime")
                .HasColumnName("pag_fecha_realizacion");
            entity.Property(e => e.PagTitulo)
                .HasMaxLength(100)
                .HasColumnName("pag_titulo");

            entity.HasOne(d => d.PagDiarioFkNavigation).WithMany(p => p.PaginaDiarios)
                .HasForeignKey(d => d.PagDiarioFk)
                .HasConstraintName("pagina_diario_ibfk_2");

            entity.HasOne(d => d.PagEmocionFkNavigation).WithMany(p => p.PaginaDiarios)
                .HasForeignKey(d => d.PagEmocionFk)
                .HasConstraintName("pagina_diario_ibfk_1");
        });

        modelBuilder.Entity<Preguntas>(entity =>
        {
            entity.HasKey(e => e.PregCodigo).HasName("PRIMARY");

            entity.ToTable("pregunta");

            entity.HasIndex(e => e.PregCategoriaFk, "preg_categoriaFK");

            entity.Property(e => e.PregCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("preg_codigo");
            entity.Property(e => e.PregCategoriaFk)
                .HasColumnType("int(40)")
                .HasColumnName("preg_categoriaFK");
            entity.Property(e => e.PregDescripcion)
                .HasMaxLength(100)
                .HasColumnName("preg_descripcion");
            entity.Property(e => e.PregEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("preg_estado_registro");

            entity.HasOne(d => d.CategoriaPregunta).WithMany(p => p.Pregunta)
                .HasForeignKey(d => d.PregCategoriaFk)
                .HasConstraintName("pregunta_ibfk_1");
        });

        modelBuilder.Entity<Programaformacion>(entity =>
        {
            entity.HasKey(e => e.ProgCodigo).HasName("PRIMARY");

            entity.ToTable("programaformacion");

            entity.HasIndex(e => e.ProgAreaFk, "prog_areaFK");

            entity.HasIndex(e => e.ProgCentroFk, "prog_centroFK");

            entity.HasIndex(e => e.ProgNivFormFk, "prog_niv_formFK");

            entity.Property(e => e.ProgCodigo)
                .HasComment("Codigo o identificador del programa de formacion")
                .HasColumnType("int(40)")
                .HasColumnName("prog_codigo");
            entity.Property(e => e.ProgAreaFk)
                .HasColumnType("int(40)")
                .HasColumnName("prog_areaFK");
            entity.Property(e => e.ProgCentroFk)
                .HasColumnType("int(40)")
                .HasColumnName("prog_centroFK");
            entity.Property(e => e.ProgEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("prog_estado_registro");
            entity.Property(e => e.ProgFormaModalidad)
                .HasComment("Tipo de modalidad de formacion")
                .HasColumnType("enum('presencial','virtual','a distancia')")
                .HasColumnName("prog_forma_modalidad");
            entity.Property(e => e.ProgModalidad)
                .HasComment("Modalidad del programa")
                .HasColumnType("enum('titulada','complementaria')")
                .HasColumnName("prog_modalidad");
            entity.Property(e => e.ProgNivFormFk)
                .HasColumnType("int(40)")
                .HasColumnName("prog_niv_formFK");
            entity.Property(e => e.ProgNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del programa")
                .HasColumnName("prog_nombre");

            entity.HasOne(d => d.Area).WithMany(p => p.Programaformacions)
                .HasForeignKey(d => d.ProgAreaFk)
                .HasConstraintName("programaformacion_ibfk_3");

            entity.HasOne(d => d.Centro).WithMany(p => p.Programaformacions)
                .HasForeignKey(d => d.ProgCentroFk)
                .HasConstraintName("programaformacion_ibfk_1");

            entity.HasOne(d => d.NivelFormacion).WithMany(p => p.Programaformacions)
                .HasForeignKey(d => d.ProgNivFormFk)
                .HasConstraintName("programaformacion_ibfk_2");
        });

        modelBuilder.Entity<Psicologo>(entity =>
        {
            entity.HasKey(e => e.PsiCodigo).HasName("PRIMARY");

            entity.ToTable("psicologo");

            entity.Property(e => e.PsiCodigo)
                .HasComment("identificador del psicologo")
                .HasColumnType("int(40)")
                .HasColumnName("psi_codigo");
            entity.Property(e => e.PsiApellido)
                .HasMaxLength(100)
                .HasComment("Apellido del psicologo")
                .HasColumnName("psi_apellido");
            entity.Property(e => e.PsiCorreoInstitucional)
                .HasMaxLength(100)
                .HasComment("correo institucional")
                .HasColumnName("psi_correo_institucional");
            entity.Property(e => e.PsiCorreoPersonal)
                .HasMaxLength(100)
                .HasComment("correo del psicologo")
                .HasColumnName("psi_correo_personal");
            entity.Property(e => e.PsiDireccion)
                .HasMaxLength(100)
                .HasComment("ubicacion de su oficina de trabajo")
                .HasColumnName("psi_direccion");
            entity.Property(e => e.PsiDocumento)
                .HasComment("documento del psicologo")
                .HasColumnType("int(11)")
                .HasColumnName("psi_documento");
            entity.Property(e => e.PsiEspecialidad)
                .HasMaxLength(100)
                .HasComment("especialidad del psicologo")
                .HasColumnName("psi_especialidad");
            entity.Property(e => e.PsiEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasComment("estado del psicologo")
                .HasColumnType("enum('activo','inactivo','suspendido')")
                .HasColumnName("psi_estado_registro");
            entity.Property(e => e.PsiFechaNac)
                .HasComment("Fecha de nacimiento")
                .HasColumnName("psi_fecha_nac");
            entity.Property(e => e.PsiFechaRegistro)
                .HasComment("Fecha en la que se le hizo el registro")
                .HasColumnType("datetime")
                .HasColumnName("psi_fecha_registro");
            entity.Property(e => e.PsiFirma)
                .HasColumnType("blob")
                .HasColumnName("psi_firma");
            entity.Property(e => e.PsiNombre)
                .HasMaxLength(100)
                .HasComment("Nombre del psicologo")
                .HasColumnName("psi_nombre");
            entity.Property(e => e.PsiPassword)
                .HasMaxLength(100)
                .HasComment("contraseña del psicologo")
                .HasColumnName("psi_password");
            entity.Property(e => e.PsiTelefono)
                .HasMaxLength(100)
                .HasComment("Numero de telefono del psicologo")
                .HasColumnName("psi_telefono");
        });

        modelBuilder.Entity<Regional>(entity =>
        {
            entity.HasKey(e => e.RegCodigo).HasName("PRIMARY");

            entity.ToTable("regional");

            entity.Property(e => e.RegCodigo)
                .HasComment("identificador del regional")
                .HasColumnType("int(40)")
                .HasColumnName("reg_codigo");
            entity.Property(e => e.RegEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("reg_estado_registro");
            entity.Property(e => e.RegNombre)
                .HasMaxLength(100)
                .HasComment("nombre del regional")
                .HasColumnName("reg_nombre");
        });

        modelBuilder.Entity<Respuestas>(entity =>
        {
            entity.HasKey(e => e.ResCodigo).HasName("PRIMARY");

            entity.ToTable("respuesta");

            entity.HasIndex(e => e.ResCategoriaFk, "res_categoriaFK");

            entity.Property(e => e.ResCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("res_codigo");
            entity.Property(e => e.ResCategoriaFk)
                .HasColumnType("int(40)")
                .HasColumnName("res_categoriaFK");
            entity.Property(e => e.ResDescripcion)
                .HasColumnType("text")
                .HasColumnName("res_descripcion");
            entity.Property(e => e.ResEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("res_estado_registro");

            entity.HasOne(d => d.CategoriaRespuesta).WithMany(p => p.Respuesta)
                .HasForeignKey(d => d.ResCategoriaFk)
                .HasConstraintName("respuesta_ibfk_1");
        });

        modelBuilder.Entity<SeguimientoAprendiz>(entity =>
        {
            entity.HasKey(e => e.SegCodigo).HasName("PRIMARY");

            entity.ToTable("seguimiento_aprendiz");

            entity.HasIndex(e => e.SegPsicologoFk, "seg_psicologoFK");

            entity.HasIndex(e => e.SegAprendizFk, "seguimiento_aprendiz_ibfk_2");

            entity.Property(e => e.SegCodigo)
                .HasComment("Identificador del seguimiento")
                .HasColumnType("int(40)")
                .HasColumnName("seg_codigo");
            entity.Property(e => e.SegAprendizFk)
                .HasComment("Aprendiz")
                .HasColumnType("int(40)")
                .HasColumnName("seg_aprendizFK");
            entity.Property(e => e.SegAreaRemitido)
                .HasMaxLength(200)
                .HasColumnName("seg_area_remitido");
            entity.Property(e => e.SegDescripcion)
                .HasMaxLength(500)
                .HasComment("Descripcion u observacion al aprendiz")
                .HasColumnName("seg_descripcion");
            entity.Property(e => e.SegEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("seg_estado_registro");
            entity.Property(e => e.SegFechaFin)
                .HasComment("Fecha final del seguimiento")
                .HasColumnName("seg_fecha_fin");
            entity.Property(e => e.SegFechaSeguimiento)
                .HasComment("Fecha de inicio del seguimiento")
                .HasColumnName("seg_fecha_seguimiento");
            entity.Property(e => e.SegMotivo)
                .HasMaxLength(400)
                .HasColumnName("seg_motivo");
            entity.Property(e => e.SegPsicologoFk)
                .HasComment("Psicologo")
                .HasColumnType("int(40)")
                .HasColumnName("seg_psicologoFK");
            entity.Property(e => e.SegRecomendaciones)
                .HasMaxLength(400)
                .HasComment("recomendaciones para el aprendiz")
                .HasColumnName("seg_recomendaciones");
            entity.Property(e => e.SegTrimestreActual)
                .HasColumnType("int(11)")
                .HasColumnName("seg_trimestre_actual");
            entity.Property(e => e.SegFirmaProfesional)
                .HasMaxLength(100)
                .HasColumnName("seg_firma_profesional");
            entity.Property(e => e.SegFirmaAprendiz)
                .HasMaxLength(100)
                .HasColumnName("seg_firma_aprendiz");

            entity.HasOne(d => d.SegAprendizFkNavigation).WithMany(p => p.SeguimientoAprendizs)
                .HasForeignKey(d => d.SegAprendizFk)
                .HasConstraintName("seguimiento_aprendiz_ibfk_4");

            entity.HasOne(d => d.SegPsicologoFkNavigation).WithMany(p => p.SeguimientoAprendizs)
                .HasForeignKey(d => d.SegPsicologoFk)
                .HasConstraintName("seguimiento_aprendiz_ibfk_3");
        });

        modelBuilder.Entity<TestGeneral>(entity =>
        {
            entity.HasKey(e => e.TestGenCodigo).HasName("PRIMARY");

            entity.ToTable("test_general");

            entity.HasIndex(e => e.TestGenApreFk, "reg_test_apreFK");

            entity.HasIndex(e => e.TestGenPsicoFk, "test_gen_psicoFK");

            entity.Property(e => e.TestGenCodigo)
                .HasColumnType("int(40)")
                .HasColumnName("test_gen_codigo");
            entity.Property(e => e.TestGenApreFk)
                .HasColumnType("int(40)")
                .HasColumnName("test_gen_apreFK");
            entity.Property(e => e.TestGenEstado)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("test_gen_estado");
            entity.Property(e => e.TestGenFechaRealiz)
                .HasColumnType("datetime")
                .HasColumnName("test_gen_fecha_realiz");
            entity.Property(e => e.TestGenPsicoFk)
                .HasColumnType("int(40)")
                .HasColumnName("test_gen_psicoFK");
            entity.Property(e => e.TestGenRecomendacion)
                .HasMaxLength(400)
                .HasColumnName("test_gen_recomendacion");
            entity.Property(e => e.TestGenResultados)
                .HasMaxLength(400)
                .HasColumnName("test_gen_resultados");

            entity.HasOne(d => d.TestGenApreFkNavigation).WithMany(p => p.TestGenerals)
                .HasForeignKey(d => d.TestGenApreFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("test_general_ibfk_1");

            entity.HasOne(d => d.TestGenPsicoFkNavigation).WithMany(p => p.TestGenerals)
                .HasForeignKey(d => d.TestGenPsicoFk)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("test_general_ibfk_2");
        });

        modelBuilder.Entity<TestPreguntas>(entity =>
        {
            entity.HasKey(e => new { e.TesRegistroFk, e.TesPregFk, e.TesRespFk })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("test_pregunta");

            entity.HasIndex(e => e.TesPregFk, "tes_pregFK");

            entity.HasIndex(e => new { e.TesRegistroFk, e.TesPregFk, e.TesRespFk }, "tes_registroFK");

            entity.HasIndex(e => e.TesRespFk, "tes_respFK");

            entity.Property(e => e.TesRegistroFk)
                .HasColumnType("int(40)")
                .HasColumnName("tes_registroFK");
            entity.Property(e => e.TesPregFk)
                .HasColumnType("int(40)")
                .HasColumnName("tes_pregFK");
            entity.Property(e => e.TesRespFk)
                .HasColumnType("int(40)")
                .HasColumnName("tes_respFK");
            entity.Property(e => e.TesEstadoRegistro)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')")
                .HasColumnName("tes_estado_registro");

            entity.HasOne(d => d.TesPregFkNavigation).WithMany(p => p.TestPregunta)
                .HasForeignKey(d => d.TesPregFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_pregunta_ibfk_1");

            entity.HasOne(d => d.TesRegistroFkNavigation).WithMany(p => p.TestPregunta)
                .HasForeignKey(d => d.TesRegistroFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_pregunta_ibfk_3");

            entity.HasOne(d => d.TesRespFkNavigation).WithMany(p => p.TestPregunta)
                .HasForeignKey(d => d.TesRespFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_pregunta_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
