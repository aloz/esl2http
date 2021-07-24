CREATE TABLE events_heartbeat_last (
	id serial NOT NULL,
	switch_id serial NOT NULL,
	event_jsonb jsonb NOT NULL,
	e_core_uuid uuid NULL GENERATED ALWAYS AS ((event_jsonb ->> 'Core-UUID'::text)::uuid) STORED,
	e_event_name text NULL GENERATED ALWAYS AS (((event_jsonb ->> 'Event-Name'::text))) STORED,
	e_event_date timestamp NULL GENERATED ALWAYS AS (to_timestamp(((event_jsonb ->> 'Event-Date-Timestamp'::text)::bigint)::double precision / 1000000::double precision)) STORED,
	CONSTRAINT events_heartbeat_last_check_event_name CHECK (((e_event_name)::text = 'HEARTBEAT'::text)),
	CONSTRAINT events_heartbeat_last_pk PRIMARY KEY (id),
	CONSTRAINT events_heartbeat_last_un_e_core_uuid UNIQUE (e_core_uuid),
	CONSTRAINT events_heartbeat_last_un_switch_id UNIQUE (switch_id)
);


ALTER TABLE events_heartbeat_last ADD CONSTRAINT events_heartbeat_last_fk_switch_core_id FOREIGN KEY (e_core_uuid) REFERENCES switches(core_uuid);
ALTER TABLE events_heartbeat_last ADD CONSTRAINT events_heartbeat_last_fk_switch_id FOREIGN KEY (switch_id) REFERENCES switches(id);