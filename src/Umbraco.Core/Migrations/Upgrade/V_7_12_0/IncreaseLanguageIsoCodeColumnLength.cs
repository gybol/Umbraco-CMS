﻿using System.Linq;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;

namespace Umbraco.Core.Migrations.Upgrade.V_7_12_0
{
    public class IncreaseLanguageIsoCodeColumnLength : MigrationBase
    {
        public IncreaseLanguageIsoCodeColumnLength(IMigrationContext context)
            : base(context)
        { }

        public override void Migrate()
        {
            var dbIndexes = SqlSyntax.GetDefinedIndexes(Context.Database)
                .Select(x => new DbIndexDefinition
                {
                    TableName = x.Item1,
                    IndexName = x.Item2,
                    ColumnName = x.Item3,
                    IsUnique = x.Item4
                }).ToArray();

            //Ensure the index exists before dropping it
            if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_umbracoLanguage_languageISOCode")))
            {
                Delete.Index("IX_umbracoLanguage_languageISOCode").OnTable("umbracoLanguage").Do();
            }

            Alter.Table("umbracoLanguage")
                .AlterColumn("languageISOCode")
                .AsString(14)
                .Nullable()
                .Do();

            Create.Index("IX_umbracoLanguage_languageISOCode")
                .OnTable("umbracoLanguage")
                .OnColumn("languageISOCode")
                .Unique()
                .Do();
        }
    }
}