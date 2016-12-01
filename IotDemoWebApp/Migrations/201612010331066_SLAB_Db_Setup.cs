namespace IotDemoWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Text;

    public partial class SLAB_Db_Setup : DbMigration
    {
        public override void Up()
        {
            StringBuilder setAnsinulls = new StringBuilder();
            StringBuilder quotedIdentifier = new StringBuilder();
            StringBuilder createTracesTypeCode = new StringBuilder();
            StringBuilder createWriteEventStoreProc = new StringBuilder();
            StringBuilder createWriteEventsStoreProc = new StringBuilder();
            StringBuilder createTracesTable = new StringBuilder();

            setAnsinulls.Append("SET ANSI_NULLS ON");

            //setAnsinulls.Append("GO");

            quotedIdentifier.Append(" SET QUOTED_IDENTIFIER ON");

            //quotedIdentifier.Append("GO");

            createTracesTypeCode.Append(" CREATE TYPE TracesType AS TABLE ([InstanceName] [nvarchar](1000),[ProviderId] [uniqueidentifier],[ProviderName] [nvarchar](500),[EventId] [int],[EventKeywords] [bigint],[Level] [int],[Opcode] [int],[Task] [int],[Timestamp] [datetimeoffset](7),[Version] [int], [FormattedMessage] [nvarchar](4000),[Payload] [nvarchar](4000),[ActivityId] [uniqueidentifier],[RelatedActivityId] [uniqueidentifier],[ProcessId] [int],[ThreadId] [int]); ");

            //createTracesTypeCode.Append("GO");

            createWriteEventStoreProc.Append(" CREATE PROCEDURE [dbo].[WriteTrace] (@InstanceName [nvarchar](1000),@ProviderId [uniqueidentifier],@ProviderName [nvarchar](500),@EventId [int],@EventKeywords [bigint],@Level [int],@Opcode [int],@Task [int],@Timestamp [datetimeoffset](7),@Version [int],@FormattedMessage [nvarchar](4000),@Payload [nvarchar](4000),@ActivityId [uniqueidentifier],@RelatedActivityId [uniqueidentifier],@ProcessId [int],@ThreadId [int],@TraceId [bigint] OUTPUT) ");

            createWriteEventStoreProc.Append("AS BEGIN SET NOCOUNT ON; INSERT INTO [Traces] ([InstanceName],[ProviderId],[ProviderName],[EventId],[EventKeywords],[Level],[Opcode],[Task],[Timestamp],[Version],[FormattedMessage],[Payload],[ActivityId],[RelatedActivityId],[ProcessId],[ThreadId]) VALUES(@InstanceName,@ProviderId,@ProviderName,@EventId,@EventKeywords,@Level,@Opcode,@Task,@Timestamp,@Version,@FormattedMessage,@Payload,@ActivityId,@RelatedActivityId,@ProcessId,@ThreadId) SET @TraceId = @@IDENTITY RETURN @TraceId END ");

            //createWriteEventStoreProc.Append("GO");

            createWriteEventsStoreProc.Append(" CREATE PROCEDURE [dbo].[WriteTraces] ( @InsertTraces TracesType READONLY ) AS BEGIN INSERT INTO [Traces] ([InstanceName],[ProviderId],[ProviderName],[EventId],[EventKeywords],[Level],[Opcode],[Task],[Timestamp],[Version],[FormattedMessage],[Payload],[ActivityId],[RelatedActivityId],[ProcessId],[ThreadId]) SELECT* FROM @InsertTraces; END");

            //createWriteEventsStoreProc.Append("GO");

            createTracesTable.Append(" CREATE TABLE [dbo].[Traces] ([id] [bigint] IDENTITY(1, 1) NOT NULL,[InstanceName] [nvarchar](1000) NOT NULL,[ProviderId] [uniqueidentifier] NOT NULL,[ProviderName] [nvarchar](500) NOT NULL,[EventId] [int] NOT NULL,[EventKeywords] [bigint] NOT NULL,[Level] [int] NOT NULL,[Opcode] [int] NOT NULL,[Task] [int] NOT NULL,[Timestamp] [datetimeoffset](7) NOT NULL,[Version] [int] NOT NULL,[FormattedMessage] [nvarchar](4000) NULL,[Payload] [nvarchar](4000) NULL,[ActivityId] [uniqueidentifier],[RelatedActivityId] [uniqueidentifier],[ProcessId] [int],[ThreadId] [int],CONSTRAINT [PK_Traces] PRIMARY KEY CLUSTERED([id] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF))");

            //createTracesTable.Append("GO");

            this.Sql(setAnsinulls.ToString());
            this.Sql(quotedIdentifier.ToString());
            this.Sql(createTracesTypeCode.ToString());
            this.Sql(createWriteEventStoreProc.ToString());
            this.Sql(createWriteEventsStoreProc.ToString());
            this.Sql(createTracesTable.ToString());
        }
        
        public override void Down()
        {
            this.Sql("DROP PROCEDURE dbo.WriteTraces; DROP PROCEDURE dbo.WriteTrace; DROP TABLE dbo.Traces; DROP TABLE dbo.TracesType; ");
        }
    }
}
