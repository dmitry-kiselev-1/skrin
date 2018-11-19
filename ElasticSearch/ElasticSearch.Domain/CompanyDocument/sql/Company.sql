
;WITH 

-- Здесь только результат фильтрации:
filter AS
(
	SELECT 
		s.ID AS us_id,
		s.*
	FROM searchdb2.dbo.union_search s
	JOIN ##elastic_loader_companyid_buffer BufferIds 
		ON BufferIds.us_id = s.ID
)

-- Здесь только отбор полей:
,company AS
(
	SELECT 
		 f.ID as us_id
		,f.us_update_date 

		--,ISNULL(f.inn, '') + '.' + ISNULL(f.ogrn, '') AS id

		,f.inn
		,f.ogrn

		,f.reg_date as ogrn_reg_date -- дата регистрации ОГРН

		,f.name AS full_name
		,f.short_name
		,f.Bones AS search_name
	
		,f.region_id2 AS region_tax_id -- Код региона по регистрации в налоговых органах
		,regions.name AS region_tax_name -- Наименование региона по регистрации в налоговых органах

		,f.okato AS region_okato_code -- ОКАТО-код
		,okato.sphinxPath AS region_okato_code_list -- ОКАТО-код и все его parent-коды (в порядке от parent до child)

		,f.okved AS sector_okved_code -- ОКВЭД-код
		,okveds.all_level_code as sector_okved_code_parent_list -- ОКВЭД-код и все его parent-коды (в порядке от child до parent)

		,gks_ID

		FROM 													filter f
		LEFT JOIN searchdb2.dbo.regions							regions
			ON regions.id = f.region_id2
		LEFT JOIN searchdb2.dbo.okato							okato
			ON okato.okato = f.okato
		LEFT JOIN searchdb2.dbo.okveds							okveds
			ON okveds.kod = f.okved
)
SELECT * FROM company;
