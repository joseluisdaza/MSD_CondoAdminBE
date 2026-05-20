# 📚 Documentación - Condominio API

Índice de toda la documentación disponible para el proyecto.

---

## 🚀 Deployment

### Render (Recomendado - GRATIS)

| Documento                                                      | Descripción                    | Tiempo     |
| -------------------------------------------------------------- | ------------------------------ | ---------- |
| **[QuickStart-Render.md](QuickStart-Render.md)** ⭐            | Guía rápida paso a paso        | 30 min     |
| **[DeployRender.md](DeployRender.md)**                         | Guía completa y detallada      | Referencia |
| **[MySQL-Gratuito.md](MySQL-Gratuito.md)**                     | Opciones de MySQL gratuito     | 15 min     |
| **[Checklist-PostDeployment.md](Checklist-PostDeployment.md)** | Verificación después de deploy | 15 min     |

### AWS

| Documento                                  | Descripción                 | Tiempo     |
| ------------------------------------------ | --------------------------- | ---------- |
| **[DeployAWS.md](DeployAWS.md)**           | Deployment en AWS ECS + RDS | 2-3 horas  |
| **[Deployment.md](Deployment.md)**         | Deployment con contenedores | Referencia |
| **[deploy-to-aws.ps1](deploy-to-aws.ps1)** | Script automatizado AWS     | -          |
| **[cleanup-aws.ps1](cleanup-aws.ps1)**     | Limpiar recursos AWS        | -          |

---

## 📖 Guía de Lectura Recomendada

### Para Deployment en Render (Primera Vez)

1. ✅ [QuickStart-Render.md](QuickStart-Render.md) - Empieza aquí
2. 📘 [MySQL-Gratuito.md](MySQL-Gratuito.md) - Configura tu database
3. ✅ [Checklist-PostDeployment.md](Checklist-PostDeployment.md) - Verifica todo

### Para Deployment en AWS

1. 📘 [DeployAWS.md](DeployAWS.md) - Guía completa
2. 🔧 Usa script: `.\deploy-to-aws.ps1`

### Para Troubleshooting

1. 🔍 [DeployRender.md - Solución de Problemas](DeployRender.md#solución-de-problemas)
2. 🔍 [MySQL-Gratuito.md - Troubleshooting](MySQL-Gratuito.md#troubleshooting)

---

## 🏗️ Arquitectura

| Documento                                                                | Descripción                          |
| ------------------------------------------------------------------------ | ------------------------------------ |
| **[Analisis_Arquitectura_Backend.md](Analisis_Arquitectura_Backend.md)** | Análisis de arquitectura del backend |

---

## 🗄️ Base de Datos

| Recurso                 | Ubicación                              |
| ----------------------- | -------------------------------------- |
| **Schema SQL**          | [Database/](Database/)                 |
| **Guía MySQL Gratuito** | [MySQL-Gratuito.md](MySQL-Gratuito.md) |

---

## 🧪 Testing

| Recurso     | Ubicación            |
| ----------- | -------------------- |
| **Pruebas** | [testing/](testing/) |

---

## 📝 Otros Recursos

| Recurso                | Ubicación                                |
| ---------------------- | ---------------------------------------- |
| **Otros documentos**   | [Others/](Others/)                       |
| **Logs de deployment** | [deployment-log.txt](deployment-log.txt) |

---

## 🎯 Decisión Rápida

### ¿Qué opción de deployment elegir?

| Necesitas                   | Usa                     |
| --------------------------- | ----------------------- |
| **Empezar gratis y rápido** | Render (30 min)         |
| **Demo o desarrollo**       | Render Free             |
| **Producción pequeña**      | Render Starter ($7/mes) |
| **Producción empresarial**  | AWS ECS (~$30/mes)      |
| **Máxima escalabilidad**    | AWS ECS                 |

### ¿Qué base de datos usar?

| Situación               | Usa               |
| ----------------------- | ----------------- |
| **Gratis, demo**        | FreeSQLDatabase   |
| **Gratis, más espacio** | db4free (200 MB)  |
| **$5 crédito inicial**  | Railway MySQL     |
| **Producción**          | AWS RDS o Railway |

---

## 🔧 Scripts Disponibles

En la raíz del proyecto:

```powershell
# Render deployment helper
.\prepare-render.ps1 -GenerateJWT
.\prepare-render.ps1 -CheckDocker
.\prepare-render.ps1 -TestLocal
.\prepare-render.ps1 -TestConnection -RenderURL "https://..."

# AWS deployment
.\Docs\deploy-to-aws.ps1

# AWS cleanup
.\Docs\cleanup-aws.ps1
```

---

## 📊 Resumen de Tiempos

| Tarea                            | Tiempo    |
| -------------------------------- | --------- |
| **Deployment Render**            | 30-45 min |
| **Deployment AWS**               | 2-3 horas |
| **Configurar MySQL gratis**      | 15 min    |
| **Verificación post-deployment** | 15 min    |

---

## ✅ Recomendación para Empezar

1. **Lee:** [QuickStart-Render.md](QuickStart-Render.md)
2. **Ejecuta:** `.\prepare-render.ps1 -GenerateJWT`
3. **Sigue:** La guía paso a paso
4. **Verifica:** [Checklist-PostDeployment.md](Checklist-PostDeployment.md)

**Total: ~1 hora** para tener tu API en producción 🚀

---

## 🆘 Ayuda

Si tienes problemas:

1. Revisar sección de troubleshooting en cada guía
2. Usar scripts de diagnóstico
3. Verificar logs en Render Dashboard
4. Consultar documentación oficial

---

**Última actualización:** Mayo 2026
