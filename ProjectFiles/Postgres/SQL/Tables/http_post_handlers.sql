CREATE TABLE http_post_handlers (
	id serial NOT NULL,
	url text NOT NULL,
	created timestamp NOT NULL DEFAULT now()::timestamp without time zone,
	last_posted timestamp NULL,
	last_statuscode int4 NULL,
	disabled timestamp NULL,
	CONSTRAINT http_post_handlers_pk PRIMARY KEY (id),
	CONSTRAINT http_post_handlers_un_url UNIQUE (url)
);
CREATE INDEX http_post_handlers_last_posted_idx ON http_post_handlers USING btree (last_posted);
CREATE INDEX http_post_handlers_last_statuscode_idx ON http_post_handlers USING btree (last_statuscode);