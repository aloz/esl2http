CREATE TABLE events (
	id bigserial NOT NULL,
	switch_id serial NOT NULL,
	event_jsonb jsonb NOT NULL,
	is_valid bool NOT NULL GENERATED ALWAYS AS ((event_jsonb ->> 'Core-UUID'::text) IS NOT NULL AND (event_jsonb ->> 'Event-Name'::text) IS NOT NULL AND (event_jsonb ->> 'Event-Date-Timestamp'::text) IS NOT NULL) STORED,
	e_core_uuid uuid NULL GENERATED ALWAYS AS ((event_jsonb ->> 'Core-UUID'::text)::uuid) STORED,
	e_event_name text NULL GENERATED ALWAYS AS (event_jsonb ->> 'Event-Name'::text) STORED,
	e_event_date timestamp NULL GENERATED ALWAYS AS (to_timestamp(((event_jsonb ->> 'Event-Date-Timestamp'::text)::bigint)::double precision / 1000000::double precision)) STORED,
	CONSTRAINT events_pk PRIMARY KEY (id)
);
CREATE INDEX events_is_valid_idx ON events USING btree (is_valid);

ALTER TABLE events ADD CONSTRAINT events_fk_switch_core_id FOREIGN KEY (e_core_uuid) REFERENCES switches(core_uuid);
ALTER TABLE events ADD CONSTRAINT events_fk_switch_id FOREIGN KEY (switch_id) REFERENCES switches(id);
