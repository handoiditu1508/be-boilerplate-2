using Hamburger.Models.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hamburger.Repository.EF.Extensions
{
    public static class MutableEntityTypeExtension
    {
        /// <summary>
        /// Dynamically ignore records with ISoftDelete.IsDeleted = True.
        /// </summary>
        /// <param name="entityData">An instance of IMutableEntityType.</param>
        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
        {
            var methodToCall = typeof(MutableEntityTypeExtension)
                .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, new object[] { });
            entityData.SetQueryFilter((LambdaExpression)filter);
            entityData.AddIndex(entityData.FindProperty(nameof(ISoftDelete.IsDeleted)));
        }

        /// <summary>
        /// Get expression for filtering out ISoftDelete.IsDeleted = True.
        /// </summary>
        /// <typeparam name="TEntity">Entity type to get expression for.</typeparam>
        /// <returns>An expression.</returns>
        private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
            return filter;
        }
    }
}
