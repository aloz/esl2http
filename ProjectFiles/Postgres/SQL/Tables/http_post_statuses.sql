CREATE TABLE http_post_statuses (
    id bigserial NOT NULL,
    event_id int8 NOT NULL,
    handler_id int4 NOT NULL,
    posted timestamp NOT NULL DEFAULT now()::timestamp without time zone,
    statuscode int4 NULL,
    reason_phrase text NULL,
    issuccess bool NOT NULL GENERATED ALWAYS AS (COALESCE(statuscode,-1) >= 200 AND COALESCE(statuscode,-1) < 300) STORED,
    CONSTRAINT http_post_statuses_pk PRIMARY KEY (id),
    CONSTRAINT http_post_statuses_un_event_id_handler_id UNIQUE (event_id, handler_id)
);
CREATE INDEX http_post_statuses_issuccess_idx ON http_post_statuses USING btree (issuccess);
CREATE INDEX http_post_statuses_statuscode_idx ON http_post_statuses USING btree (statuscode);

ALTER TABLE http_post_statuses ADD CONSTRAINT http_post_statuses_fk_events FOREIGN KEY (event_id) REFERENCES events(id);
ALTER TABLE http_post_statuses ADD CONSTRAINT http_post_statuses_fk_http_post_handlers FOREIGN KEY (handler_id) REFERENCES http_post_handlers(id);