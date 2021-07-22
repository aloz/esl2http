-- Anthony Minessale Thu, 25 Mar 2010 06:13:50 -0700
-- core_uuid:
-- unique per running instance of FS, it changes each time you restart.

CREATE TABLE switches (
	id serial NOT NULL,
	core_uuid uuid NOT NULL,
	created timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	last_event timestamp NULL DEFAULT now()::timestamp without time zone,
	CONSTRAINT switches_pk PRIMARY KEY (id),
	CONSTRAINT switches_un_core_uuid UNIQUE (core_uuid)
);