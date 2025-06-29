PGDMP      ,                }            CorporateMessengerDB    16.3    16.3     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    41075    CorporateMessengerDB    DATABASE     �   CREATE DATABASE "CorporateMessengerDB" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
 &   DROP DATABASE "CorporateMessengerDB";
                postgres    false                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
                pg_database_owner    false            �           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                   pg_database_owner    false    5            �            1259    41116 
   chat_users    TABLE     Y   CREATE TABLE public.chat_users (
    user_id uuid NOT NULL,
    chat_id uuid NOT NULL
);
    DROP TABLE public.chat_users;
       public         heap    postgres    false    5            �            1259    41108    chats    TABLE       CREATE TABLE public.chats (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    name character varying(100) NOT NULL,
    is_group boolean DEFAULT false NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    avatar bytea,
    creator uuid
);
    DROP TABLE public.chats;
       public         heap    postgres    false    5    5    5            �            1259    41131    messages    TABLE     p  CREATE TABLE public.messages (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    text character varying,
    sender_id uuid NOT NULL,
    chat_id uuid NOT NULL,
    sent_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    file_url character varying,
    file_name character varying,
    file_type character varying,
    file_size bytea
);
    DROP TABLE public.messages;
       public         heap    postgres    false    5    5    5            �            1259    41099    users    TABLE     �  CREATE TABLE public.users (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    name character varying NOT NULL,
    surname character varying NOT NULL,
    patronymic character varying,
    avatar bytea NOT NULL,
    current_position character varying NOT NULL,
    role character varying
);
    DROP TABLE public.users;
       public         heap    postgres    false    5    5    5            �          0    41116 
   chat_users 
   TABLE DATA           6   COPY public.chat_users (user_id, chat_id) FROM stdin;
    public          postgres    false    218   .       �          0    41108    chats 
   TABLE DATA           P   COPY public.chats (id, name, is_group, created_at, avatar, creator) FROM stdin;
    public          postgres    false    217   �       �          0    41131    messages 
   TABLE DATA           t   COPY public.messages (id, text, sender_id, chat_id, sent_at, file_url, file_name, file_type, file_size) FROM stdin;
    public          postgres    false    219   �,       �          0    41099    users 
   TABLE DATA           ~   COPY public.users (id, username, password, created_at, name, surname, patronymic, avatar, current_position, role) FROM stdin;
    public          postgres    false    216   �;       >           2606    41120    chat_users chat_users_pkey 
   CONSTRAINT     f   ALTER TABLE ONLY public.chat_users
    ADD CONSTRAINT chat_users_pkey PRIMARY KEY (user_id, chat_id);
 D   ALTER TABLE ONLY public.chat_users DROP CONSTRAINT chat_users_pkey;
       public            postgres    false    218    218            <           2606    41115    chats chats_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.chats
    ADD CONSTRAINT chats_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.chats DROP CONSTRAINT chats_pkey;
       public            postgres    false    217            @           2606    41139    messages messages_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.messages DROP CONSTRAINT messages_pkey;
       public            postgres    false    219            8           2606    41105    users users_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    216            :           2606    41107    users users_username_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);
 B   ALTER TABLE ONLY public.users DROP CONSTRAINT users_username_key;
       public            postgres    false    216            A           2606    41126 "   chat_users chat_users_chat_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.chat_users
    ADD CONSTRAINT chat_users_chat_id_fkey FOREIGN KEY (chat_id) REFERENCES public.chats(id) ON DELETE CASCADE;
 L   ALTER TABLE ONLY public.chat_users DROP CONSTRAINT chat_users_chat_id_fkey;
       public          postgres    false    218    4668    217            B           2606    41121 "   chat_users chat_users_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.chat_users
    ADD CONSTRAINT chat_users_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;
 L   ALTER TABLE ONLY public.chat_users DROP CONSTRAINT chat_users_user_id_fkey;
       public          postgres    false    216    4664    218            C           2606    41145    messages messages_chat_id_fkey    FK CONSTRAINT     }   ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_chat_id_fkey FOREIGN KEY (chat_id) REFERENCES public.chats(id);
 H   ALTER TABLE ONLY public.messages DROP CONSTRAINT messages_chat_id_fkey;
       public          postgres    false    219    4668    217            D           2606    41140     messages messages_sender_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_sender_id_fkey FOREIGN KEY (sender_id) REFERENCES public.users(id);
 J   ALTER TABLE ONLY public.messages DROP CONSTRAINT messages_sender_id_fkey;
       public          postgres    false    219    216    4664            �   �   x����m1��^x�(~{qBQ��K�2;9@���y�%�]�X�L�Q"Q����?��o�?�]&��OBX�0�k�N�F��x?1�k&}����i���1Ph�GNh�9�r�<lz�d.��P{�^���ڧTq`)�6�e�9E��b)ڐvG�0�ŜgMG���}߿'��'      �   �  x��Knd�u�ǬUhnd#'"N�"����s�=�P�&^�G�ځdA@˶�֎��,��X�d��[BK��If2�ވ���q�����咊/I9\z���¨9�ۮw�����������w������w�z\H�/����o����o����r����m�{/�w�$K�|��N�ݏs��yy������k��fWxd��qQ�Qy4/n��C/^y�k����o^{�~�g�:��!�jh���w�х��)(��{I�u��l6o�y}ʘ�Ϲ����g�������M�~s�pV{Ъ�s�����7�>^'/���@'6���m�3��E������eG�ٵ8UҬyH?���/��4��e�~�Pk�u=�ɷ�U+�Qc}� �F�^?|�}�m�_)Ēޟ�J�ط�c��_��P�[i����]B��y2!����-~r~q2Z��r��g�M� c���_3@,��֓1yi��,�5�8��J��G�o����^3�߲�a��j7y�O����R|X�+���N��5�&驎0�L���s�����1�ֺ�^;o�g��ki�kY!!Ju���jռ�X;�Z&Ŝ!x�x$*/L�:����T�ѳ��G���$�s�������G�eBBc[C^�$����[^�d� 7��SV7�_ȉ^V�}3�|.���B Fa�}kOe��_0b�����k��cSh_�!~�8)/�F1s����g��n�׍�2�����Qfr�ʼ�ulm�:���)���8d�=	�S���&}�$�xC��/{3�k}�Г'm�.!���s<�+P��/�S�"/`��4����C��z�+L�>�+���G^j�HLP�7D��,��9�l���1Q��"G l��U7~���J������kԑCn�Ӳ%)��v���S�ƝL��Q%찜�� 3�,$`�<	�����&���KPBWj�l�YDީ8����>?}_�L��@m^(�R�N� =k�,�RB���ʕ4ݡPU4�×2	�r%y�{g���l�0���*�e8��-�q�9	w҆8@eJF��$���y=Q�,(��YY�RsJKXߴd%ŖK.KK�قfRU��|�[�I�!� E�iQ�H�N ;�����P�8ŀXs\����B���v��Y�����l�i��0����.�Rl��<��=�F.�J֞9D'	�����*��%��M��z�F��d<�i��aMIb�8L`z��J�S='�J�(DP���j���~/*���Uʦδ�YK���#t�A�4�=��� m7֩����NPB�ê|�mdȭ�Y�;�R�b
]��r@��D�TL���c�P5���S=+�z¤l4�Y��A�$��/n�E�_;F�|���g�pe��a>�lF -OE��d)���@��]�VƂ�?��23��X�S�)�.�pO�L8��t�^�tˬ��)ӑLr�)1
pA�䝝Pi�QN({$J�ch�.�7��	&�#vP�r���Ǒ��v�N\�@)6`�ǉS��*��I��lB��BN p�T�"=�tV
>n��q����߄�鐰,_�asa��{���c&l�Z�� �eUrبP�eb1Ev��h���4bbՀ.�H$��jgҔa�Bs�5l,a��jU����m����tV�K�ğbI�a8��i���(��o���:<1g�'�ݑr�ؗ<�ZR5,O�!n1!P� ����q�֦yA��7�"SQ�<V����r�=
�UZo��࿶U�ǲ�w["��Ê}��O�t��۸-3M�ki��4 Ӷ��}i��҆)3��7iɑ�W�b�e���H&�1�%Ҳ��F$#�$�ug{SQ˕�*�2��l�6q�\�PG]��b�bk�s��Bp;L}	$���P*���z��$��O�V&�XoM��-^�T���v���X��t��8m�V��yRD�N0�
�������FE#��A�7�
�+����f�W����@�#C ��4(a����q�n�����j&�CK�o&!�2�F� �(?���{PBvT� �8)1PIᇮ��+�}�3���2'A�W�r��2���m0iD���!m��Ƥd"�$��� ��TDY �A�re��v�M�o��"i�@��cat���iԆ��x�߰u�I|	��2h�}�z*��k�#E���dgl5����d�9%?m�h24��4o3�+�S�9����H%Hj���u�S�P���h����nJ�6��%�`i
 Qh��P����$����s���'m�QHWXbq��q튎-?:r9�Le7����(<�%�SKX��ۻ�BT���	�[^󢦠�aa���ɻ�8G�W��}�PTY P�:z[�19�� �K���	Kd�DP��̳�t/�ŝVu�>�B�+���:���3�}�?�s��.qo����}��_sU�	V�^q���'�U#���J����_����<�5��S)�.	c���5>���36�%��6��j���T��W�r���U��D_�/��f~{��ʭ��M���<P?_��>���g��qL�_?��<ƕͼ����;������z��ј�3.S��7���1.��?lc�Y6J�� ��]�v~vj�u:h/�	��h�w�d �qm۹j@�Z*�1��(�/m��P;�~�z_�K�~C���.�g5�� (�Pz���<�޾�Y�X��v���{ .mo�%
�<��
�̰���m�i�C���,�4� ��|���:h�!��!��L/@qr]\���>Bҋ+�� �=7����^oDH�V�-C�q�������FJx'����%Cۃfی+��S �� ؜�ve�Bd�k����D��J��>��kC��
4h��׵�!S��ݮ����Æ�Q0$���&�O�]�v�����A�D���j�F":m��Nڂ�Ğ���2Y|)H�Z;�%;#����Nm�+Ba"�fԺ+>ۓ�V'�4RZ��w�R�R�Fh;�� ,sv(��p!�N�4@��	s����7��bt�����6�e�+u��JѸ5���W����IqFIKl��Z7ƩЬ���v�֚��'\�ˬXz�0��;���e��w2�bQ��Q���""4�笅e�^|��$=.��Z1Vǲ���m�L�V��܋i*4z��s����e)E�v�����ƅ����	ۄ���9�<3+�J;�"�eN(NO�6��
?�n3$}B�e��ʷY���m��T��xn������Jv� ��j[���Y�Z���P�8	C��X(�*�Qp���!AK��x��C�PeZ�3�lse�G,]�lf7���%#�V�F�2R�,�W�ĢY.��Mc�m�L�K��ʎ,�A�/Y��މN"s�1�+�X�����Xe	�w��>T82|��rQ�_�5�R]	d��(���	Cm��9�n)�t��rb���U;gG�s�#u�Tqn�ɦh��n	�ὸW�;�:��P�*h*���I
���{��$���d�M&�o��#���$pb=t�
*�J�����ଶQ���HA��G���=�E�Rnj�M���	���V�G�%f�XW��g��7:�>4ʵ�Fs���qi:�Er���.��	� U7ۿ|�A�߆�)��;t���_w��C�����A��q:�ދq:�>>N�9�f��;t���tН��Aw:�N��;t���tН�����;t��n���:t�0��Aw:�N��;t���tН��A�G�z^vq�"T�Km�_\��Y����g���-t�[�����i�;-t��������}|��g��B�^��B��qZ���7;N�i�;-t���Н��BwZ�N�i�;-t������>O�i�;-t���}X�i���y���BwZ�N�i�;-t����}}��	3�j�l���J���K���%����|���������������?�������߿�������ڿxM�*���_�=��3��7�
��7f$.�^�v���]�FZ�����wo��~�*Ư���?�FC�)|�0�}��͛�^���      �   �  x��K�$���3��Z`�A2��!o�4`�)ؐ-a$^���`���!��_����T�G@UUwee1�����,���Z�n���Tf�k}�ݩg�pBֶ^|������~�����H�[>E�r�z��Ļ�μ���tJ�כ��׭ڋb��N�7�|Ӈ��C�/�����2[�*}�D_%���ܵ\��2�����VcfT�ҋ�>�����W��W����������W��_~�ɧ}}~��3B����Nw���v�+Z���������/~�O��/��ۿ�/���ԟ��?�/���~βsvA�v-ڴ\��^��@��q�����r��Py�`�sQ��xvѰq뤫**����.+f9���d�9L1�K��[�I�W
qE���y{;�F����&WR4������s�pa���Ǭ"�B�_���RZ��os���C�{�����o����W�>:�<\$���]��������2ͫ��y�<-^RS���,u>���Z/m?�I�(ФfK���얈z���Z͔�9��c<
�diI�}����r�y��g�̡�ڻ�y4 㜞�:=}�}:{N�
�!+�n�;й1�.�� �5N��Q\�zKzN/�e�ڣ����Ǌ�S�s����9�]1�'��DYL9�������'���J�u�6�z4�;�_��^/�W�9��]�>刬��X��눑�옙J���[�r��'��>��b��
3�O��qi�#B� sP�ڙuQ
8�n�q[��N?o,VH	K}O}b�,�T���6�E�Qo�.�=��~3O첫���%��nN�iT�8�h°��΄��-��%�Ph��9�7A�ۭ^�ݵ��D�)V8m�}�A��	�r���Co�n}�z���q���Y���� DN�A���5��怬��;X3�1OY���¯�_��_�2�O0�t1/hp�vFǇ*1SVE��iIR��o�?p��ab�����٫Jkk����8)S��#"�j�C\Űm�Z&�F���m�X��r�2��4�a.%�+�t	T�%�x�BӨ���&��<+�P����.H�V����au,��\�4�&�9.���Xy���(J�G���*Գ���(%�O�P/m���JU�<]L�ueN)s�J>	�}��s�Z��rύ�r"AF/�Σ�Q�Ɇ�t�L���AAK��ŜWC=�`0����$ �������[-
KJ��R��X�'9ʤ��=~Wz�E� $"� |V�:RG�8uE�����3��`8o"��Z����4X�P�κ�Y�e�N��އ�P�mQ�b�l���T:XZ�^%CD D�|�⨊9�I&d �R�ޚ���O�5}R\�I�2k*����6B�e����UVG�0�V=�v��d�e�����0��:�+��5%�g�JiZg�b�Q��
�3�ێ�����UIZ}��ϳAw��	�����1��iq�fm�f�"b�R5t��S���	iR<oT�|�
{����K�h����Go�*^*1^�-ds�8�\N�Rz��H�`]&����bF|�X&��\��^�8�'{!{���Wݺ�v��u�I)�w#G!.X�9���4�zIb�X��Z��v�4w'�Pf����Ȝ#�_k�� Q�w����q�T	���`Pf��"�v߸I-׸��J��F�'Lq1�K���MJ�Z�3�[ZE<,rn0C6�Q!��w*��2R�v��BXv�fcU����U<����-Erb��-;Xd�@g�64�@��lk\)�Z�d��['!罂W���gx�d��d�r��``�p��J�B���	f@���3	����
Hx�'��&`�v2��/�����"�0�����!v=r��=�ߩ�����UDĲ��["��ÊePĦP8û�m:�L�:�`Ӕ'b���Dj?���>ݘ�?�R ���b�#�AEGD$��V��GHMI#��j��#��T�z�R.���]�Ex����zCC{�Sr
��(����^f'A�T
s��i�l^>	T�z�Y�5��-Q�͖@;WC��R:PN4;� �����d�V�̨�F�� �#�Y}e|��a6,ɘ��W�^M@���a^k�
�9²gA v�yR(�&'&�/Bq���K7hˊFIϭ�ϱ��7�	�R�s�=F��<ɈFʢ��*	b��!E*)�04x��O	�����E�NT. ��'�f�&��"��{#�f�}.J)M��f&�<�`E`ȆPL
W�sV��h������UI�F��3�cq�'�?Z�� 6��?���;�/%ՠSN-�D���i�6ktQć/ ���$��q�1}�|:���l�����ҿ4B�5:�&��J��c�0J�P�&�x���P7I�F;_M+:LA$� ���2��ޭSMRl�W�ԛql�(���dSB�ac�� f��vh��ݱ��@�x�(����&2��2�h*�� �O�-���)�mzZ�� ��2�s�x��;���ꆁ��9�&��R���1� -�3%�
�WY��{c~H��U��I*�~{g6�7�'�������]���^������|}<���\���g��3?:#��>��,ʣ�?�z��yk�'_F����8���қ��k�.�cd��f?ڡ��?�[!���Jn�B�I��ķ'���Jn�}?䑲��������/���8�~��f��y��'���p_����\Z<��	�e�������3�e�x��7�ީf`	~d�y�����}��B���r�+����}��C9��;W��jC9f����a���ۀ�o��{q	�o��]W��d5�h�O�r�u�T��-c��Ց��RXk�	���������g�����zA8�G��i�At�����d�3�:���W�8X��f��$Dz�9$�= �{�?l;��>L!��l�����҃�}Ci]���|�Fu�?kA�G+�	W�~��Bnm3hs�[��%VL��Y�d	�ɬήM��I�L��}4|ЩIF_����e9��ű|�*Ks6��#�m�jZf�����k���䊡���	jb��[5�"Q[>��&}#~��I�SЈ���R`
:������11����S�����0�2%ĕ��F̎���@ÑgλJ�hG$e�UV�(B�Y=W���9����B�;�� ��J,����s����1P`Hb�w�D�յ.鈬�j�X�j߯~��w�\�������iȬ�T��x����'�8۽�6v(�U�Fϖ�d:��������1Y���cB�L	��rƨ2�6�H�Gͽ��c�g���w:�f���>(����݆@1�]�bSy��Ա�6i�����TVaH�P$�����*�jg�a�k�!�y�K}W����Pl�l@Q�ke�Evi�u�S+���So}+�WYn�|�b���"�p�����
��J��Tv�EI�T��� |ӌa��@�͕5�=ӲA3���M��|z]�1��c) R�;r�=�����:}��G
9�Ms�'���d$�t��{'����i"Ww�_9���+[��e�ۇ�FF�B4v�f��KX�*-�jO�a*�@9Z(��p0��]��v��F٥!GT�N��$3��n�+n��˨��%��������W�r-��(D���{�@M����|��G�F~��$qHr=�
�@%A^����EhV�(#�)R�w���q�'u��4�M���/���-3�����l�[b�C�Kj��^�������+��      �      x��ͮe�U���O�9�P�j�W� ��1	�<�O��!3s�0D(!!� $vhLl^��7�v�n���\߶�ȉ�>���g��U�V�����R��J�I��,�������?j
;��[���|�{��U�r����/��ݿ�}��?��ߏ?��w��;������||���!�[�K�7����ps����\~v���������o.�Ɨ�|ȋ/���/��q��~|�����=���9'N�&���/�rZ��s�v���ir�"#���L.�J���M�W��D�r��ʢRx�g��d�(��������O>�ꛯRe�����72���I�53��l\	��'������7;��>���s�y1��K+�U�Ӫ�6�竎�X�C����Z�8Љ�?��mtF��I0�/��<^�H)�f�8kG����-9T��������T��:I메� n�P�ɪ�/�^���#�X��=S	%�-a���f�kt+�v�j٣�������qv�m�>�>;-�G�r�C+�;бP�J�� ��w`ֽ>qi��L�1�8��r���K�wI�vZ��B�nd�+��M�쓅��|�1�ME�J�J���f(Q{���{þ���Rk�X�}���^;m�Yg������{�\���R]h��ZK�k�t-�bN�l��D��a��PG�z'J=i�{�I*�$����U�
e��\c	�(�����\�5��~ҫ׫
B©����/�D/+��_�r����c_D���1��{]�52el�1D6����q�[��\��L���ºҠ.�۠i_�eFת�k?��F���5y��A��`9u�x爪��F^�d��ؾG!l>. �����W����)��>��������UQ=`Z������Ԟ#�r����^3K�s��T�p�C$��E��@جAJ�ؕw�,���6]��6^���S�2-Z2�lW�aL)���t
P���~�儦^g�IH��yT7Aݑ/h��	KQB/��ٴ3��cv��/��y�^����@n���x�sǚ��5v�D*!�Tu���l��*%�Cr��K����7!�wz�n̖3���b%�lG���cOѫװci�T&e�jAASZioE�GCݺP ^2t�B?� ����Z�� �VrRPRh)��J�)���Lj)}uߥ�$� �C� |%�H�N���!;����Q�����k`"�n+T"b0Ѯv_i�gէ�')[J��>�����.R!��2��CzH�X�
Q{�� ���6L��+�pN:4)D�m1��&�	����5)]F�b0��t�c�Ehl�**�#A�fN��l���^d�f/�7y�eZ�Rs��2�i�i��z��*��h[e[�J�r����F�!������(5.�(+�]�%Ѩ��Y2���d�ٿ�w�V�M�F��9�/da�|��xa,j��}������h�����f��T0:M�҃�z\�"�2;0t�+1ⳈU$�S�ڲg�dK�c�����2�[g%�N,�$��Z�G.h���#*�)��uO�����z�mgb�ŖL(#tP�r��Y0d�a���-��Q�u���q�T1�
�`P� ��"1 5f��3������Ę�O^��7.kA:$�Kj�<L|�#L�6�����p�Yp��� ~+�eUbب��"������5�!�<>��G��XT ��jg�n�|s�5�X����5m��Ÿ���P],�+�$�0\�劗�E�8/X�|��OP�#�;X_�L ki-~	��H�Մ@M�`lz*D��6�
�k�t�LE�<ӎ;�5��c��.ӊ�V����",��<���f,�$6���^�oö�4Y]+�4����-��6>1�Sf$�o#��n��5���!�8��K�%!TōFd;HH����B�+-��(�Y�mb)�Qy�B����N�
��0�����F�A�T
r�i�&�X��H`ej^��b�Q,�bE���X��;1���`I0'�mL��J_�T<O��Ȩ�F�V�A��}f|��èh�1��F\az9b�Z��-U`a>zP�H�VV$
����鋐X@Ǩ�e�BJ�5�ϾE�/q�T�c����y��.;��E����ɤ�CW1��b}�3���2'N(+��p	���64 o�X��6�hc�2p�iJrD�i�*"�, � q�<F.v�&yW��<+NZI�qx����-�� ��~�7l��ĿWN�L⡞O������"�,pљ�Xz�F2����m4�Ha���OөՌG.��H&��l3능��&9Y�&6G!o��mxQ4Loa
 )��&��P���������9{+���(�+Vb�q�$�vE�1��k�A�mn34R�9�}	���ga�nf�UeZ^# �&�����)�m�[�ш�]� Α��at��\#���\Go;&���t͕1;n�̤2T�i�7��|P�iY��I&*.vgֹ�1O�Q������ܘ�u��;�1ʧ��]��.>����yT����G)P�M����/��B��)��[�_����������͏[�����K�u�$�wɕ����>��W�����$W�}=`䞴�����;���ڼ���Wf�/�q��g;|r<Ow�u_�W������3.���7���1.�ĺ�1�J6J�� ����v~vl�u:h/�	��h�w�& �qm۹j@��+�1��({.m�&_;�~�z_�[�~C����Ef5�� (7�{��<����̚-�B;kps�= ����b@wy��Fo��J�"�b�>;���!���B/�bH/wH8d7�3P�X�F��Hzv��@��������q�ZfK�hX0=ཀ���H����a�TC�#'h�/�6#��ޞ�d�mY lN`�3��j!2ӵM�h#A7�j�\�û��`p���Q��M"��f8dL�`�������0w�P�cAކT�D�O�]��������A�D�޶jF�e����	=:(-�G,��,	�������(�!�& t�wr30�@ 
L���������eG�G�+KpG(e�Y�o0B�Y݇�0��C���;�� ��&L��	��s�����a`Plʷۀ��9���\�.��m��^M�3J�C�6k�,N�fyD]��֔�p}/[ź�C����՛�l(Ӷ���s����,�*x�sV²{�ҹ7j�#ǖ��1����u���j;�{1L�Foz���t]�R�v�}Te�BS�d�qۈ�`�ɇ43�j�"�tN(NS�Uc�?;�"�>��:�v���ۈ�6��N(jm���FM�c�kE{\[h�-��*�aY�Z��HP��i�B���(��ĳ�1��J>)Nr��!�o�°�d����H�Xey��=췦KG�-�a1�c*D$�'Q����E[�ൻ�1�6���-	�*��L�A�/]���I�x��2��+[���sQVe)�w�}�pd�(@c�\
<� ָJu��򣘌#��&L|Tj�N�v;x�Iv��hԐ�=�NT����$#v�T1n��L����"�;�{1�nO��\���O�Ĝl���q̺��MjZ���d�K���E���5�u�12�*1��5:Kg��2)�7I
�^$(dܞ� �H"�tS3f����W��-��:�ۖ�bT{�zs�����{G����On߳ʷ.��<�|l�q����Ϟ��V��ҼrW���uyGQ�Ol��/.�� &�Oodux��ĥ�(�����gI�Y�w_�8K�tq��|�%y�<Β�b�%y/gI�y�ڎ�$�,�;K�Β��$�,�;K�Β��$�,�;K�Β��$�7�}��%ygI�>K�>��Y��[�>K�Β��$�,�;K�Β��$�,ɻV�����/��|��o�{r����.�\~q���ꨌ�����{|����^>�}��Ǘ���n.�q|y��G������c���͟�w�7�d��!�
>��������ӳ�A������κ��xs�Y�w���n����{�q�����{�8�����v�u{g��Y�w��u{g��Y�w��u{g��Y�w��u{���n��;���Y���κ�ߚ�Y�w�� /   u{g��Y�w��u{g�ޕ��?��_��'�����k����[o���8t�     