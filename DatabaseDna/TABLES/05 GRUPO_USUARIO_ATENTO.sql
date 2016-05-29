-- Create table
CREATE TABLE DNAONLINE.GRUPO_USUARIO_ATENTO
(
  ID_GRUPO_USUARIO_ATENTO NUMBER(8)     NOT NULL,
  NM_GRUPO_USUARIO_ATENTO VARCHAR2(100) NOT NULL,
  DS_GRUPO_USUARIO_ATENTO VARCHAR2(100) NOT NULL
)
TABLESPACE TS_DNAONLINE;
-- Create/Recreate primary, unique and foreign key constraints 
ALTER TABLE DNAONLINE.GRUPO_USUARIO_ATENTO
  ADD CONSTRAINT PK_GRUPO_USUARIO_ATENTO 
  PRIMARY KEY (ID_GRUPO_USUARIO_ATENTO);