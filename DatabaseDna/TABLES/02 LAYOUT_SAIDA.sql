-- Create table
CREATE TABLE DNAONLINE.LAYOUT_SAIDA
(
  ID_LAYOUT_SAIDA NUMBER(8)     NOT NULL,
  NM_LAYOUT_SAIDA VARCHAR2(100) NOT NULL
)
TABLESPACE TS_DNAONLINE;
-- Create/Recreate primary, unique and foreign key constraints 
ALTER TABLE DNAONLINE.LAYOUT_SAIDA
  ADD CONSTRAINT PK_LAYOUT_SAIDA 
  PRIMARY KEY (ID_LAYOUT_SAIDA);