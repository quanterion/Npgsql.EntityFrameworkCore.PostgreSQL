﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Npgsql specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class NpgsqlModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the model to use a sequence-based hi-lo pattern to generate values for properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting PostgreSQL.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="name"> The name of the sequence. </param>
        /// <param name="schema">The schema of the sequence. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForNpgsqlUseSequenceHiLo(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string name = null,
            [CanBeNull] string schema = null)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var model = modelBuilder.Model;

            name = name ?? NpgsqlModelAnnotations.DefaultHiLoSequenceName;

            if (model.Npgsql().FindSequence(name, schema) == null)
            {
                modelBuilder.HasSequence(name, schema).IncrementsBy(10);
            }

            model.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.SequenceHiLo;
            model.Npgsql().HiLoSequenceName = name;
            model.Npgsql().HiLoSequenceSchema = schema;

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the model to use the PostgreSQL SERIAL feature to generate values for properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting PostgreSQL. This is the default
        ///     behavior when targeting PostgreSQL.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForNpgsqlUseSerialColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var property = modelBuilder.Model;

            property.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.SerialColumn;
            property.Npgsql().HiLoSequenceName = null;
            property.Npgsql().HiLoSequenceSchema = null;

            return modelBuilder;
        }

        public static ModelBuilder HasPostgresExtension(
            [NotNull] this ModelBuilder modelBuilder,
            [NotNull] string name)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotEmpty(name, nameof(name));

            modelBuilder.Model.Npgsql().GetOrAddPostgresExtension(name);
            return modelBuilder;
        }

        public static ModelBuilder HasDatabaseTemplate(
            [NotNull] this ModelBuilder modelBuilder,
            [NotNull] string templateDatabaseName)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotEmpty(templateDatabaseName, nameof(templateDatabaseName));

            modelBuilder.Model.Npgsql().DatabaseTemplate = templateDatabaseName;
            return modelBuilder;
        }
    }
}
