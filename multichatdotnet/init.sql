create table providers (
	id text primary key, -- gemini/nvidia/openrouter
	display_name text not null,
	is_enabled integer not null,
	api_key text not null, -- encrypted
	base_url text not null,
	registration_url text not null,
	extra_headers text, -- json
	created_at timestamp default CURRENT_TIMESTAMP
);

create table models (
	id text primary key, -- gemini-2.5-flash
	provider_id text references providers (id),
	is_enabled integer not null,
	supports_tool_calling integer not null,
	supports_vision integer not null,
	max_context_tokens integer default 0,
	max_tokens_per_minute integer default 0,
	max_requests_per_minute integer default 0,
	is_free_tier integer,
	cost_per_million_input_tokens integer,
	cost_per_million_outputs_tokens integer,
	created_at timestamp default CURRENT_TIMESTAMP
);

create table chat_master (
	id integer primary key, 
	title text  NOT NULL DEFAULT 'New Conversation',
	is_pinned integer DEFAULT 0,
	system_prompt text,
	model_id text references models (id),
	created_at timestamp default CURRENT_TIMESTAMP,
	last_modified_at timestamp  DEFAULT CURRENT_TIMESTAMP
);

create table chat_detail (
	id integer primary key,
	chat_id integer references chat_master(id),
	role text, -- user/assistant
	content text, -- Hello champ!
	model_id text references models(id), -- model used for answering this
	is_error integer default 0,
	error_details text,
	created_at timestamp default CURRENT_TIMESTAMP
);

CREATE INDEX idx_models_provider ON models (provider_id);
CREATE INDEX idx_chat_master_sorting ON chat_master (is_pinned DESC, last_modified_at DESC);
CREATE INDEX idx_chat_detail_thread ON chat_detail (chat_id);