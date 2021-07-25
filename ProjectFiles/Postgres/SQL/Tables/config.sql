CREATE TABLE config (
	id smallserial NOT NULL,
	timeout_s_http int4 NULL,
	CONSTRAINT config_check_id CHECK ((id = 1)),
	CONSTRAINT config_check_timeout_s_http CHECK ((timeout_s_http >= '-1'::integer))
);