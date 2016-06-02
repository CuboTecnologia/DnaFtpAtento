-- Create table
CREATE TABLE DNAONLINE.USUARIO_ATENTO_GRUPO_USUARIO
(
  DS_LOGIN                VARCHAR2(20)  NOT NULL,
  ID_GRUPO_USUARIO_ATENTO NUMBER(8)     NOT NULL
)
TABLESPACE TS_DNAONLINE;
-- Create/Recreate primary, unique and foreign key constraints 
ALTER TABLE DNAONLINE.USUARIO_ATENTO_GRUPO_USUARIO
  ADD CONSTRAINT PK_USUARIO_ATENTO_GRUPO 
  PRIMARY KEY (DS_LOGIN, ID_GRUPO_USUARIO_ATENTO);
  
ALTER TABLE DNAONLINE.USUARIO_ATENTO_GRUPO_USUARIO
  ADD CONSTRAINT USUARIO_ATENTO_GRUPO_FK2 
  FOREIGN KEY (ID_GRUPO_USUARIO_ATENTO)
  REFERENCES DNAONLINE.GRUPO_USUARIO_ATENTO (ID_GRUPO_USUARIO_ATENTO);
  
ALTER TABLE DNAONLINE.USUARIO_ATENTO_GRUPO_USUARIO
  ADD CONSTRAINT USUARIO_ATENTO_GRUP_FK1 
  FOREIGN KEY (DS_LOGIN)
  REFERENCES DNAONLINE.USUARIO_ATENTO (DS_LOGIN);