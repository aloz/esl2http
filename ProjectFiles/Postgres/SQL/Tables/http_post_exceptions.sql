CREATE TABLE http_post_exceptions (
	id bigserial NOT NULL,
	event_id int8 NOT NULL,
	handler_id int4 NOT NULL,
	posted timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	exception_text text NULL,
	CONSTRAINT http_post_exceptions_pk PRIMARY KEY (id)
);

ALTER TABLE http_post_exceptions ADD CONSTRAINT http_post_exceptions_fk_events FOREIGN KEY (event_id) REFERENCES events(id);
ALTER TABLE http_post_exceptions ADD CONSTRAINT http_post_exceptions_fk_http_post_handlers FOREIGN KEY (handler_id) REFERENCES http_post_handlers(id);