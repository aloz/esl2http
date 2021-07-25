CREATE TABLE http_post_exceptions (
	id bigserial NOT NULL,
	http_post_status_id int8 NOT NULL,
	posted timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	exception_text text NULL,
	CONSTRAINT http_post_exceptions_pk PRIMARY KEY (id)
);
CREATE INDEX http_post_exceptions_http_post_status_id_idx ON http_post_exceptions USING btree (http_post_status_id);
CREATE INDEX http_post_exceptions_posted_idx ON http_post_exceptions USING btree (posted);

ALTER TABLE http_post_exceptions ADD CONSTRAINT http_post_exceptions_fk FOREIGN KEY (id) REFERENCES http_post_statuses(id);