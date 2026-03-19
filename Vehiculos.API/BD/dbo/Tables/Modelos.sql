CREATE TABLE [dbo].[Modelos] (
    [Id]      UNIQUEIDENTIFIER NOT NULL,
    [IdMarca] UNIQUEIDENTIFIER NOT NULL,
    [Nombre]  VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_Modelos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Modelos_Marcas] FOREIGN KEY ([IdMarca]) REFERENCES [dbo].[Marcas] ([Id])
);

