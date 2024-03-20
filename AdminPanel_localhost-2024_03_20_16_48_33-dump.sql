--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: AdminPanel; Type: DATABASE; Schema: -; Owner: admin
--

CREATE DATABASE "AdminPanel" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE "AdminPanel" OWNER TO admin;

\connect "AdminPanel"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Games; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."Games" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "Name" character varying(32) NOT NULL,
    "Description" character varying(32)
);


ALTER TABLE public."Games" OWNER TO admin;

--
-- Name: COLUMN "Games"."Id"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Games"."Id" IS 'Идентификатор сущности.';


--
-- Name: COLUMN "Games"."Name"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Games"."Name" IS 'Наименование.';


--
-- Name: COLUMN "Games"."Description"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Games"."Description" IS 'Описание.';


--
-- Name: Passwords; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."Passwords" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "EncryptedValue" character varying(32) NOT NULL,
    "UserId" uuid NOT NULL
);


ALTER TABLE public."Passwords" OWNER TO admin;

--
-- Name: COLUMN "Passwords"."Id"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Passwords"."Id" IS 'Идентификатор сущности.';


--
-- Name: COLUMN "Passwords"."EncryptedValue"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Passwords"."EncryptedValue" IS 'Зашифрованное значение.';


--
-- Name: COLUMN "Passwords"."UserId"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Passwords"."UserId" IS 'Внешний ключ записи пользователя.';


--
-- Name: Rights; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."Rights" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "Name" character varying(32) NOT NULL,
    "Description" character varying(32) NOT NULL,
    "GameId" uuid NOT NULL
);


ALTER TABLE public."Rights" OWNER TO admin;

--
-- Name: COLUMN "Rights"."Id"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Rights"."Id" IS 'Идентификатор сущности.';


--
-- Name: COLUMN "Rights"."Name"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Rights"."Name" IS 'Наименование.';


--
-- Name: COLUMN "Rights"."Description"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Rights"."Description" IS 'Описание.';


--
-- Name: COLUMN "Rights"."GameId"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Rights"."GameId" IS 'Внешний ключ записи игры.';


--
-- Name: Users; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."Users" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "Email" character varying(32) NOT NULL,
    "Nickname" character varying(20) NOT NULL
);


ALTER TABLE public."Users" OWNER TO admin;

--
-- Name: COLUMN "Users"."Id"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Users"."Id" IS 'Идентификатор сущности.';


--
-- Name: COLUMN "Users"."Email"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Users"."Email" IS 'Электронная почта.';


--
-- Name: COLUMN "Users"."Nickname"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."Users"."Nickname" IS 'Имя пользователя.';


--
-- Name: UsersRights; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."UsersRights" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "UserId" uuid NOT NULL,
    "RightId" uuid NOT NULL
);


ALTER TABLE public."UsersRights" OWNER TO admin;

--
-- Name: COLUMN "UsersRights"."Id"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."UsersRights"."Id" IS 'Идентификатор сущности.';


--
-- Name: COLUMN "UsersRights"."UserId"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."UsersRights"."UserId" IS 'Внешний ключ записи пользователя.';


--
-- Name: COLUMN "UsersRights"."RightId"; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public."UsersRights"."RightId" IS 'Внешний ключ записи права.';


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO admin;

--
-- Data for Name: Games; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."Games" VALUES ('8cf5bb26-d3ef-44e1-938f-42125aa0aeab', 'ExampleGame', 'Игра для примера');


--
-- Data for Name: Passwords; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."Passwords" VALUES ('319af27b-24e4-4e61-ba80-5836bf394345', '1234', '87f6247d-594e-4d31-821d-11ad535da8f5');


--
-- Data for Name: Rights; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."Rights" VALUES ('3a66503b-e934-4256-95af-094005d1e974', 'example_right', 'Право для примера', '8cf5bb26-d3ef-44e1-938f-42125aa0aeab');


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."Users" VALUES ('87f6247d-594e-4d31-821d-11ad535da8f5', 'test@email.com', 'ExampleUser');


--
-- Data for Name: UsersRights; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."UsersRights" VALUES ('48fde486-9a7a-45b0-8ec0-935a2986a1c1', '87f6247d-594e-4d31-821d-11ad535da8f5', '3a66503b-e934-4256-95af-094005d1e974');


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: admin
--

INSERT INTO public."__EFMigrationsHistory" VALUES ('20240219083227_InitialCreate', '7.0.16');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20240304021639_AddRightsAndGames', '7.0.16');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20240317104606_AddUsersRights', '7.0.16');


--
-- Name: Games PK_Games; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Games"
    ADD CONSTRAINT "PK_Games" PRIMARY KEY ("Id");


--
-- Name: Passwords PK_Passwords; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Passwords"
    ADD CONSTRAINT "PK_Passwords" PRIMARY KEY ("Id");


--
-- Name: Rights PK_Rights; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Rights"
    ADD CONSTRAINT "PK_Rights" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: UsersRights PK_UsersRights; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."UsersRights"
    ADD CONSTRAINT "PK_UsersRights" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Games_Name; Type: INDEX; Schema: public; Owner: admin
--

CREATE UNIQUE INDEX "IX_Games_Name" ON public."Games" USING btree ("Name");


--
-- Name: IX_Nickname; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX "IX_Nickname" ON public."Users" USING btree ("Nickname");


--
-- Name: IX_Passwords_UserId; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX "IX_Passwords_UserId" ON public."Passwords" USING btree ("UserId");


--
-- Name: IX_Rights_GameId; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX "IX_Rights_GameId" ON public."Rights" USING btree ("GameId");


--
-- Name: IX_Rights_Name; Type: INDEX; Schema: public; Owner: admin
--

CREATE UNIQUE INDEX "IX_Rights_Name" ON public."Rights" USING btree ("Name");


--
-- Name: IX_UsersRights_RightId; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX "IX_UsersRights_RightId" ON public."UsersRights" USING btree ("RightId");


--
-- Name: IX_UsersRights_UserId; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX "IX_UsersRights_UserId" ON public."UsersRights" USING btree ("UserId");


--
-- Name: Passwords FK_Passwords_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Passwords"
    ADD CONSTRAINT "FK_Passwords_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Rights FK_Rights_Games_GameId; Type: FK CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."Rights"
    ADD CONSTRAINT "FK_Rights_Games_GameId" FOREIGN KEY ("GameId") REFERENCES public."Games"("Id") ON DELETE CASCADE;


--
-- Name: UsersRights FK_UsersRights_Rights_RightId; Type: FK CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."UsersRights"
    ADD CONSTRAINT "FK_UsersRights_Rights_RightId" FOREIGN KEY ("RightId") REFERENCES public."Rights"("Id") ON DELETE CASCADE;


--
-- Name: UsersRights FK_UsersRights_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public."UsersRights"
    ADD CONSTRAINT "FK_UsersRights_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

