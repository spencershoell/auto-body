using Microsoft.OData.ModelBuilder;
using Shoell.Autobody.Models;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Services
{
    public static class EdmBaseConfiguration
    {
        public static EntityTypeConfiguration<TEntity> ConfigureForBaseModel<TEntity>(this EntityTypeConfiguration<TEntity> model, string ns = "Shoell.Autobody")
            where TEntity : BaseModel, new()
        {
            var type = typeof(TEntity);
            var entityType = type.GetProperty("EntityType")?.GetValue(null) as EntityType;

            model.Ignore(e => e.DateArchived);
            model.Ignore(e => e.ArchivedBy);
            model.Ignore(e => e.ArchivedById);
            model.Ignore(e => e.DateDeleted);
            model.Ignore(e => e.DeletedById);
            model.Ignore(e => e.DeletedBy);

            model.Function(Log.EntityType.EntitySet)
                .ReturnsCollectionFromEntitySet<Log>(Log.EntityType.EntitySet)
                .Namespace = $"{ns}.Functions";

            model.Action("Archive")
                .Namespace = $"{ns}.Actions";

            model.Action("Restore")
               .Namespace = $"{ns}.Actions";

            model.Action("Recover")
               .Namespace = $"{ns}.Actions";

            model.Action("Purge")
               .Namespace = $"{ns}.Actions";

            model
               .Collection
               .Function("Recycle")
               .ReturnsCollectionFromEntitySet<TEntity>(entityType?.Type.AsPlural())
               .Namespace = $"{ns}.Functions";

            return model;
        }

        public static EntityTypeConfiguration<TEntity> ConfigureForRepositoryEntity<TEntity>(this EntityTypeConfiguration<TEntity> model, string ns = "Shoell.Autobody")
                where TEntity : class, IBaseModel
        {
            model.Ignore(e => e.DateArchived);
            model.Ignore(e => e.ArchivedById);
            model.Ignore(e => e.DateDeleted);
            model.Ignore(e => e.DeletedById);
            model.Ignore(e => e.DateDeleted);
            model.Ignore(e => e.DeletedById);

            model.Function(Log.EntityType.EntitySet)
                .ReturnsCollectionFromEntitySet<Log>(Log.EntityType.EntitySet)
                .Namespace = $"{ns}.Functions";

            return model;
        }
    }
}
