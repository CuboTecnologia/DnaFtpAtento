CREATE OR REPLACE PACKAGE BODY DNA.PKG_CONTROLE_ARQUIVO_ATENTO
AS
    PROCEDURE ATUALIZAR_STATUS_DOWNLOAD
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE,
        P_NM_ARQUIVO_UNZIP   IN CONTROLE_ARQ_ATENTO.NM_ARQUIVO_UNZIP%TYPE,
        P_NM_ARQUIVO_SAIDA   IN CONTROLE_ARQ_ATENTO.NM_ARQUIVO_SAIDA%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 2,
            NM_ARQUIVO_UNZIP           = P_NM_ARQUIVO_UNZIP,
            NM_ARQUIVO_SAIDA           = P_NM_ARQUIVO_SAIDA,
            DT_INICIO_EXECUCAO         = SYSDATE,
            IS_ENVIO_EMAIL             = 'N'
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE ATUALIZAR_STATUS_IMPORTACAO
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 3
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE ATUALIZAR_STATUS_EXTRACAO
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 4
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE ATUALIZAR_STATUS_EXPORTACAO
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 5
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE ATUALIZAR_STATUS_UPLOAD
    (
        P_ID_CONTROLE         IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE,
        P_NM_ARQUIVO_DOWNLOAD IN CONTROLE_ARQ_ATENTO.NM_ARQUIVO_DOWNLOAD%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 6,
            NM_ARQUIVO_DOWNLOAD        = P_NM_ARQUIVO_DOWNLOAD,
            DT_TERMINO_EXECUCAO        = SYSDATE
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;
    
    PROCEDURE ATUALIZAR_ENVIO_EMAIL
    (
        P_ID_CONTROLE         IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            IS_ENVIO_EMAIL             = 'S'
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;
    
    PROCEDURE ATUALIZAR_STATUS_ERRO_LAYOUT
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 99
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE ATUALIZAR_STATUS_EXPURGADO
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE
    )
    IS
    BEGIN
        UPDATE
            CONTROLE_ARQ_ATENTO
        SET
            CD_STATUS_CONTROLE_ARQUIVO = 98
        WHERE
            ID_CONTROLE = P_ID_CONTROLE;
        COMMIT;
    END;

    PROCEDURE VERIFICAR_LOGIN_USUARIO
    (
        P_DS_LOGIN        IN USUARIO_ATENTO.DS_LOGIN%TYPE,
        P_DS_SENHA        IN USUARIO_ATENTO.DS_SENHA%TYPE,
        RETORNO_LOGIN     OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         OPEN RETORNO_LOGIN FOR
         
          SELECT  
                USUARIO_ATENTO.DS_LOGIN
                ,DS_SENHA
                ,DS_CONFIRMACAO_SENHA
                ,NM_USUARIO
                ,DS_EMAIL
                ,CD_TIPO_USUARIO
                ,NM_GRUPO_USUARIO_ATENTO
          FROM 
                USUARIO_ATENTO
          JOIN
                USUARIO_ATENTO_GRUPO_USUARIO
          ON
                USUARIO_ATENTO_GRUPO_USUARIO.DS_LOGIN = USUARIO_ATENTO.DS_LOGIN
          JOIN
                GRUPO_USUARIO_ATENTO
          ON
                GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO = USUARIO_ATENTO_GRUPO_USUARIO.ID_GRUPO_USUARIO_ATENTO
          WHERE 
                USUARIO_ATENTO.DS_LOGIN = P_DS_LOGIN
          AND 
                DS_SENHA = P_DS_SENHA;
      
    END;
    
    PROCEDURE LISTAR_CONTROLE_ARQUIVO
    (
        P_DS_LOGIN                  IN  VARCHAR2,
        RETORNO_CONTROLE_ARQUIVO    OUT SYS_REFCURSOR
    )
    IS
        V_CD_TIPO_USUARIO DNA.USUARIO_ATENTO.CD_TIPO_USUARIO%TYPE;
    BEGIN
    
         SELECT
             CD_TIPO_USUARIO
         INTO
             V_CD_TIPO_USUARIO
         FROM
             DNA.USUARIO_ATENTO  
         WHERE
             DS_LOGIN = P_DS_LOGIN;
    
         OPEN 
              RETORNO_CONTROLE_ARQUIVO 
         FOR
         
          SELECT ID_CONTROLE
                 ,NM_LAYOUT_ENTRADA
                 ,NM_LAYOUT_SAIDA
                 ,NM_ARQUIVO_ENTRADA
                 ,NM_ARQUIVO_ENTRADA_ORIGINAL
                 ,STATUS_CONTROLE_ARQ_ATENTO.CD_STATUS_CONTROLE_ARQUIVO
                 ,DS_STATUS_CONTROLE_ARQUIVO
                 ,DT_REGISTRO
                 ,DT_INICIO_EXECUCAO
                 ,DT_TERMINO_EXECUCAO
                 ,USUARIO_ATENTO.DS_LOGIN
                 ,USUARIO_ATENTO.NM_USUARIO
                 ,QT_ITENS_RECEBIDOS
                 ,QT_ITENS_EXPORTADOS
                 ,QT_ENRIQUECIDO_ENDERECO
                 ,QT_ENRIQUECIDO_FONE
                 ,QT_ENRIQUECIDO_CELULAR
                 ,QT_ENRIQUECIDO_EMAIL
          FROM
                 CONTROLE_ARQ_ATENTO
          JOIN
                 LAYOUT_ENTRADA
          ON
                 LAYOUT_ENTRADA.ID_LAYOUT_ENTRADA = CONTROLE_ARQ_ATENTO.ID_LAYOUT_ENTRADA
          JOIN
                 LAYOUT_SAIDA
          ON
                 LAYOUT_SAIDA.ID_LAYOUT_SAIDA = CONTROLE_ARQ_ATENTO.ID_LAYOUT_SAIDA
          JOIN
                 STATUS_CONTROLE_ARQ_ATENTO
          ON
                 STATUS_CONTROLE_ARQ_ATENTO.CD_STATUS_CONTROLE_ARQUIVO = CONTROLE_ARQ_ATENTO.CD_STATUS_CONTROLE_ARQUIVO
          JOIN
                 USUARIO_ATENTO
          ON
                 USUARIO_ATENTO.DS_LOGIN = CONTROLE_ARQ_ATENTO.DS_LOGIN
          WHERE
                (V_CD_TIPO_USUARIO = 'A'
                 OR CONTROLE_ARQ_ATENTO.DS_LOGIN IN (SELECT DISTINCT GRP.DS_LOGIN
                                                     FROM   DNA.USUARIO_ATENTO_GRUPO_USUARIO GRP
                                                     JOIN   DNA.USUARIO_ATENTO USU
                                                     ON     USU.DS_LOGIN = GRP.DS_LOGIN
                                                     WHERE  USU.DS_LOGIN = P_DS_LOGIN))
          ORDER BY
                 ID_CONTROLE DESC;
    END;
    
    PROCEDURE ATUALIZAR_CONTROLE_ARQUIVO
    (
        P_DT_REGISTRO                 IN CONTROLE_ARQ_ATENTO.DT_REGISTRO%TYPE,
        P_ID_LAYOUT_ENTRADA           IN CONTROLE_ARQ_ATENTO.ID_LAYOUT_ENTRADA%TYPE,
        P_ID_LAYOUT_SAIDA             IN CONTROLE_ARQ_ATENTO.ID_LAYOUT_SAIDA%TYPE,
        P_NM_ARQUIVO_ENTRADA          IN CONTROLE_ARQ_ATENTO.NM_ARQUIVO_ENTRADA%TYPE,
        P_NM_ARQUIVO_ENTRADA_ORIGINAL IN CONTROLE_ARQ_ATENTO.NM_ARQUIVO_ENTRADA_ORIGINAL%TYPE,
        P_DS_LOGIN                    IN CONTROLE_ARQ_ATENTO.DS_LOGIN%TYPE
    )
    IS
    BEGIN
         INSERT INTO CONTROLE_ARQ_ATENTO
                     (ID_CONTROLE                
                     ,DT_REGISTRO                
                     ,ID_LAYOUT_ENTRADA          
                     ,ID_LAYOUT_SAIDA            
                     ,NM_ARQUIVO_ENTRADA         
                     ,NM_ARQUIVO_ENTRADA_ORIGINAL
                     ,CD_STATUS_CONTROLE_ARQUIVO 
                     ,DS_LOGIN)
         VALUES
                     (SQ_ARQ_ATENTO.NEXTVAL
                     ,P_DT_REGISTRO                
                     ,P_ID_LAYOUT_ENTRADA          
                     ,P_ID_LAYOUT_SAIDA            
                     ,P_NM_ARQUIVO_ENTRADA         
                     ,P_NM_ARQUIVO_ENTRADA_ORIGINAL
                     ,1
                     ,P_DS_LOGIN);
         COMMIT;
    END;
    
    PROCEDURE CRIAR_USUARIO_ATENTO
    (
        P_DS_LOGIN                    IN USUARIO_ATENTO.DS_LOGIN%TYPE,
        P_DS_SENHA                    IN USUARIO_ATENTO.DS_SENHA%TYPE,
        P_DS_CONFIRMACAO_SENHA        IN USUARIO_ATENTO.DS_CONFIRMACAO_SENHA%TYPE,
        P_NM_USUARIO                  IN USUARIO_ATENTO.NM_USUARIO%TYPE,
        P_DS_EMAIL                    IN USUARIO_ATENTO.DS_EMAIL%TYPE,
        P_CD_GRUPOS                   IN VARCHAR2,
        P_CD_TIPO_USUARIO             IN USUARIO_ATENTO.CD_TIPO_USUARIO%TYPE
    )
    IS
    BEGIN
         INSERT INTO USUARIO_ATENTO
                     (DS_LOGIN            
                     ,DS_SENHA            
                     ,DS_CONFIRMACAO_SENHA
                     ,NM_USUARIO          
                     ,DS_EMAIL            
                     ,CD_TIPO_USUARIO)
         VALUES
                     (P_DS_LOGIN
                     ,P_DS_SENHA
                     ,P_DS_CONFIRMACAO_SENHA
                     ,P_NM_USUARIO
                     ,P_DS_EMAIL
                     ,P_CD_TIPO_USUARIO);
                     
         INSERT INTO USUARIO_ATENTO_GRUPO_USUARIO
                     (DS_LOGIN
                     ,ID_GRUPO_USUARIO_ATENTO)
         SELECT 
                     P_DS_LOGIN,
                     REGEXP_SUBSTR(P_CD_GRUPOS,'[^,]+', 1, LEVEL) 
         FROM 
                     DUAL
         CONNECT BY 
                     REGEXP_SUBSTR(P_CD_GRUPOS, '[^,]+', 1, LEVEL) IS NOT NULL;           
         
    END;
    
    PROCEDURE BAIXAR_ARQUIVO_POR_ID
    (
        P_ID_CONTROLE                 IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE,
        RETORNO_POR_ID                OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         OPEN 
              RETORNO_POR_ID 
         FOR
         
         SELECT 
                NM_ARQUIVO_DOWNLOAD
         FROM
                CONTROLE_ARQ_ATENTO
         WHERE
                ID_CONTROLE = P_ID_CONTROLE;
    END;       
    
    PROCEDURE LISTAR_GRUPO_USUARIO
    (
        RETORNO_GRUPO_USUARIO    OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         OPEN
             RETORNO_GRUPO_USUARIO
         FOR
         
         SELECT
               ID_GRUPO_USUARIO_ATENTO
               ,NM_GRUPO_USUARIO_ATENTO
               ,DS_GRUPO_USUARIO_ATENTO
         FROM
               GRUPO_USUARIO_ATENTO
         ORDER BY 
               ID_GRUPO_USUARIO_ATENTO ASC;
    END;
    
    PROCEDURE ATUALIZAR_GRUPO_USUARIO
    (
        P_NM_GRUPO_USUARIO_ATENTO     IN GRUPO_USUARIO_ATENTO.NM_GRUPO_USUARIO_ATENTO%TYPE,
        P_DS_GRUPO_USUARIO_ATENTO     IN GRUPO_USUARIO_ATENTO.DS_GRUPO_USUARIO_ATENTO%TYPE
    )
    IS
    BEGIN
         INSERT INTO GRUPO_USUARIO_ATENTO
                     (ID_GRUPO_USUARIO_ATENTO
                     ,NM_GRUPO_USUARIO_ATENTO
                     ,DS_GRUPO_USUARIO_ATENTO)
              VALUES
                     (SQ_GRUPO_USUARIO_ATENTO.NEXTVAL
                     ,P_NM_GRUPO_USUARIO_ATENTO
                     ,P_DS_GRUPO_USUARIO_ATENTO);
              COMMIT; 
    END;
    
    PROCEDURE GERAR_RELATORIO
    (
        P_ID_CONTROLE        IN CONTROLE_ARQ_ATENTO.ID_CONTROLE%TYPE,
        RETORNO_RELATORIO    OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         OPEN 
              RETORNO_RELATORIO 
         FOR
         
          SELECT ID_CONTROLE
                 ,NM_LAYOUT_ENTRADA
                 ,NM_LAYOUT_SAIDA
                 ,NM_ARQUIVO_ENTRADA_ORIGINAL
                 ,DS_STATUS_CONTROLE_ARQUIVO
                 ,DT_REGISTRO
                 ,DT_INICIO_EXECUCAO
                 ,DT_TERMINO_EXECUCAO
                 ,NM_ARQUIVO_DOWNLOAD
                 ,NM_USUARIO
                 ,QT_ITENS_RECEBIDOS
                 ,QT_ITENS_EXPORTADOS
                 ,QT_ENRIQUECIDO_ENDERECO
                 ,QT_ENRIQUECIDO_FONE
                 ,QT_ENRIQUECIDO_CELULAR
                 ,QT_ENRIQUECIDO_EMAIL
          FROM
                 CONTROLE_ARQ_ATENTO
          JOIN
                 LAYOUT_ENTRADA
          ON
                 LAYOUT_ENTRADA.ID_LAYOUT_ENTRADA = CONTROLE_ARQ_ATENTO.ID_LAYOUT_ENTRADA
          JOIN
                 LAYOUT_SAIDA
          ON
                 LAYOUT_SAIDA.ID_LAYOUT_SAIDA = CONTROLE_ARQ_ATENTO.ID_LAYOUT_SAIDA
          JOIN
                 STATUS_CONTROLE_ARQ_ATENTO
          ON
                 STATUS_CONTROLE_ARQ_ATENTO.CD_STATUS_CONTROLE_ARQUIVO = CONTROLE_ARQ_ATENTO.CD_STATUS_CONTROLE_ARQUIVO
          JOIN
                 USUARIO_ATENTO
          ON
                 USUARIO_ATENTO.DS_LOGIN = CONTROLE_ARQ_ATENTO.DS_LOGIN
          WHERE
                 ID_CONTROLE = P_ID_CONTROLE;
    END;
    
    PROCEDURE LISTAR_GRUPO_USUARIO_CKL
    (
        P_TIPO_USUARIO        IN VARCHAR2,
        P_DS_LOGIN            IN VARCHAR2,
        RETORNO_GRUPO_USUARIO OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         IF P_TIPO_USUARIO = 'A' THEN
         
         OPEN 
              RETORNO_GRUPO_USUARIO 
         FOR
         
         SELECT 
                ID_GRUPO_USUARIO_ATENTO
                ,NM_GRUPO_USUARIO_ATENTO
                ,DS_GRUPO_USUARIO_ATENTO
         FROM   
                GRUPO_USUARIO_ATENTO;
         
         ELSE
         
         OPEN 
              RETORNO_GRUPO_USUARIO 
         FOR
         
             SELECT 
                    ID_GRUPO_USUARIO_ATENTO
                    ,NM_GRUPO_USUARIO_ATENTO
                    ,DS_GRUPO_USUARIO_ATENTO
             FROM   
                    GRUPO_USUARIO_ATENTO
             WHERE
                    ID_GRUPO_USUARIO_ATENTO IN 
                    (SELECT 
                            ID_GRUPO_USUARIO_ATENTO 
                    FROM 
                            USUARIO_ATENTO_GRUPO_USUARIO 
                    WHERE 
                            DS_LOGIN = P_DS_LOGIN);
         END IF;
    END;
    
    PROCEDURE LISTAR_USUARIOS
    (
        P_DS_LOGIN        IN  USUARIO_ATENTO.DS_LOGIN%TYPE,
        RETORNO_USUARIOS  OUT SYS_REFCURSOR
    )
    IS
    BEGIN
         
         OPEN 
              RETORNO_USUARIOS
         FOR
        
          SELECT 
              USUARIO_ATENTO.DS_LOGIN
             ,USUARIO_ATENTO.NM_USUARIO
             ,USUARIO_ATENTO.DS_EMAIL
             ,GRUPO_USUARIO_ATENTO.NM_GRUPO_USUARIO_ATENTO
             ,GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO
          FROM 
              USUARIO_ATENTO
          JOIN
              USUARIO_ATENTO_GRUPO_USUARIO
          ON
              USUARIO_ATENTO_GRUPO_USUARIO.DS_LOGIN = USUARIO_ATENTO.DS_LOGIN
          JOIN
              GRUPO_USUARIO_ATENTO
          ON
              GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO = USUARIO_ATENTO_GRUPO_USUARIO.ID_GRUPO_USUARIO_ATENTO
          WHERE
              USUARIO_ATENTO_GRUPO_USUARIO.ID_GRUPO_USUARIO_ATENTO IN (SELECT G.ID_GRUPO_USUARIO_ATENTO
                                                                            FROM USUARIO_ATENTO_GRUPO_USUARIO A,
                                                                                 GRUPO_USUARIO_ATENTO         G
                                                                           WHERE A.ID_GRUPO_USUARIO_ATENTO = G.ID_GRUPO_USUARIO_ATENTO
                                                                             AND A.DS_LOGIN = P_DS_LOGIN)                                         
          ORDER BY USUARIO_ATENTO.DS_LOGIN;
     
    END;
  
    PROCEDURE EDITAR_USUARIO
    (
        P_NM_USUARIO        IN USUARIO_ATENTO.NM_USUARIO%TYPE,
        P_DS_LOGIN          IN USUARIO_ATENTO.DS_LOGIN%TYPE,
        RETORNO_USUARIO     OUT SYS_REFCURSOR
    )
    IS
    BEGIN
    
    OPEN
          RETORNO_USUARIO
    FOR
    
    SELECT 
                NM_USUARIO
                ,DS_EMAIL
                ,DS_LOGIN 
        FROM 
                USUARIO_ATENTO
        WHERE
                DS_LOGIN = P_DS_LOGIN;
    END;
    
    PROCEDURE ATUALIZAR_USUARIO
    (
        P_DS_LOGIN           IN USUARIO_ATENTO.DS_LOGIN%TYPE,
        P_NM_NOVO_USUARIO    IN VARCHAR2,
        P_DS_NOVO_EMAIL      IN VARCHAR2,
        P_CD_GRUPOS          IN VARCHAR2
    )
    IS
    BEGIN
         
         UPDATE
               USUARIO_ATENTO
         SET
               NM_USUARIO = P_NM_NOVO_USUARIO,
               DS_EMAIL = P_DS_NOVO_EMAIL
         
         WHERE
               DS_LOGIN = P_DS_LOGIN;
               
         COMMIT;
         
         DELETE FROM
                    USUARIO_ATENTO_GRUPO_USUARIO
         WHERE
                    DS_LOGIN = P_DS_LOGIN;
                    
         COMMIT;
         
         INSERT INTO
                    USUARIO_ATENTO_GRUPO_USUARIO
                    (DS_LOGIN
                    ,ID_GRUPO_USUARIO_ATENTO)
         SELECT
                    P_DS_LOGIN
                    ,REGEXP_SUBSTR(P_CD_GRUPOS,'[^,]+',1,LEVEL)
         FROM
                    DUAL
         CONNECT BY
                    REGEXP_SUBSTR(P_CD_GRUPOS,'[^,]+',1,LEVEL) IS NOT NULL;
         
    END;
    
    PROCEDURE LISTAR_MEMBROS_GRUPO
    (
        P_ID_GRUPO           IN GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO%TYPE,
        RETORNO_GRUPO        OUT SYS_REFCURSOR
    )
    IS
    BEGIN
    
          OPEN
              RETORNO_GRUPO
          FOR
          
          SELECT
                GRUPO_USUARIO_ATENTO.NM_GRUPO_USUARIO_ATENTO
                ,GRUPO_USUARIO_ATENTO.DS_GRUPO_USUARIO_ATENTO
                ,USUARIO_ATENTO.DS_LOGIN
                ,USUARIO_ATENTO.NM_USUARIO
                ,USUARIO_ATENTO.DS_EMAIL
          FROM
                GRUPO_USUARIO_ATENTO
          JOIN
                USUARIO_ATENTO_GRUPO_USUARIO
          ON
                USUARIO_ATENTO_GRUPO_USUARIO.ID_GRUPO_USUARIO_ATENTO = GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO
          JOIN 
                USUARIO_ATENTO
          ON
                USUARIO_ATENTO.DS_LOGIN = USUARIO_ATENTO_GRUPO_USUARIO.DS_LOGIN
          WHERE
                GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO = P_ID_GRUPO;
    END;    
    
    PROCEDURE EDITAR_GRUPO
    (
        P_NM_GRUPO          IN GRUPO_USUARIO_ATENTO.NM_GRUPO_USUARIO_ATENTO%TYPE,
        P_ID_GRUPO          IN GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO%TYPE,
        RETORNO_GRUPO       OUT SYS_REFCURSOR
    )
    IS
    BEGIN
    
    OPEN
          RETORNO_GRUPO
    FOR
    
    SELECT 
            NM_GRUPO_USUARIO_ATENTO
            ,DS_GRUPO_USUARIO_ATENTO
            ,ID_GRUPO_USUARIO_ATENTO
    FROM 
            GRUPO_USUARIO_ATENTO
    WHERE
            ID_GRUPO_USUARIO_ATENTO = P_ID_GRUPO;
    END;
    
    PROCEDURE ATUALIZAR_GRUPO
    (
        P_ID_GRUPO           IN GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO%TYPE,
        P_NM_NOVO_GRUPO      IN VARCHAR2,
        P_DS_NOVA_DESCRICAO  IN VARCHAR2
    )
    IS
    BEGIN
         
         UPDATE
               GRUPO_USUARIO_ATENTO
         SET
               NM_GRUPO_USUARIO_ATENTO = P_NM_NOVO_GRUPO,
               DS_GRUPO_USUARIO_ATENTO = P_DS_NOVA_DESCRICAO
         
         WHERE
               ID_GRUPO_USUARIO_ATENTO = P_ID_GRUPO;
               
         COMMIT;
         
    END;
    
    PROCEDURE VERIFICAR_USUARIO_CONTROLE
    (
        P_DS_LOGIN           IN   CONTROLE_ARQ_ATENTO.DS_LOGIN%TYPE,
        RETORNO_USUARIO      OUT  SYS_REFCURSOR
    )
    IS 
    BEGIN
         OPEN
             RETORNO_USUARIO
         FOR
         
         SELECT 
               DS_LOGIN
         FROM
               CONTROLE_ARQ_ATENTO CAA
         WHERE
               DS_LOGIN = P_DS_LOGIN;
    END;
    
    
    PROCEDURE DELETAR_USUARIO
    (
        P_DS_LOGIN           IN USUARIO_ATENTO.DS_LOGIN%TYPE
    )
    
    IS
    BEGIN
    
          DELETE FROM
                     USUARIO_ATENTO_GRUPO_USUARIO UAGU
          WHERE
                     UAGU.DS_LOGIN = P_DS_LOGIN;
          COMMIT;
          
                     
          DELETE FROM
                     USUARIO_ATENTO UA
          WHERE
                     UA.DS_LOGIN = P_DS_LOGIN;
          COMMIT;
          
    END;
    
    PROCEDURE VERIFICAR_USUARIO_GRUPO
    (
        P_CD_GRUPO           IN USUARIO_ATENTO_GRUPO_USUARIO.ID_GRUPO_USUARIO_ATENTO%TYPE
        ,RETORNO_CD_GRUPO    OUT SYS_REFCURSOR
    )
    
    IS
    BEGIN
    
         OPEN
             RETORNO_CD_GRUPO
         FOR
         
         SELECT
               DS_LOGIN
         FROM
               USUARIO_ATENTO_GRUPO_USUARIO UAGU
         WHERE
               UAGU.ID_GRUPO_USUARIO_ATENTO = P_CD_GRUPO;
    
    END;
    
    PROCEDURE DELETAR_GRUPO
    (
        P_CD_GRUPO         IN GRUPO_USUARIO_ATENTO.ID_GRUPO_USUARIO_ATENTO%TYPE
    )
    IS
    BEGIN
    
         DELETE FROM
                    GRUPO_USUARIO_ATENTO GUA
         WHERE
                    GUA.ID_GRUPO_USUARIO_ATENTO = P_CD_GRUPO;
         COMMIT;
    
    END;
    
    PROCEDURE LISTAR_GRUPOS
    (
        P_DS_LOGIN            IN USUARIO_ATENTO.DS_LOGIN%TYPE,
        RETORNO_GRUPO         OUT SYS_REFCURSOR
    )
    
    IS
    BEGIN
    
    OPEN
        RETORNO_GRUPO
    FOR
    
         SELECT 
                UAGU.ID_GRUPO_USUARIO_ATENTO 
         FROM 
                USUARIO_ATENTO_GRUPO_USUARIO UAGU 
         WHERE 
                UAGU.DS_LOGIN = P_DS_LOGIN;
    
    END;
    
END;