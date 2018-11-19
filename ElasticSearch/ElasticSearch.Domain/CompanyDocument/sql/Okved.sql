
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

	-- Здесь список дополнительных ОКВЭД со строкой всех родителей:
	,sector_okved_code_additional_parent_list AS
	(
		SELECT 
			 filter.us_id 
			,union_okved.okved
			,okveds.all_level_code as sector_okved_code_parent_list -- ОКВЭД-код и все его parent-коды (в порядке от child до parent)
			--,ROW_NUMBER() OVER (PARTITION BY filter.us_id ORDER BY filter.okved) AS id_number
			--,COUNT(*) OVER (PARTITION BY filter.us_id) AS id_count
			FROM													filter
			JOIN searchdb2.dbo.union_okved							union_okved -- 100 млн
				ON union_okved.us_id = filter.us_id
			JOIN searchdb2.dbo.okveds								okveds
				ON okveds.kod = union_okved.okved
	)
	
	/*
	-- Здесь список дополнительных ОКВЭД со строкой всех родителей - из XML (вариант 1):
	,sector_okved_code_additional_parent_list_xml_v1 AS
	(
		SELECT
			filter.us_id,
			STUFF
			(
				(
					SELECT 
						', ' + t.sector_okved_code_parent_list AS codes
						FROM											sector_okved_code_additional_parent_list t
						WHERE t.us_id = filter.us_id
						FOR XML PATH(''), TYPE
				).value('.', 'varchar(max)')  
				, 1, 2, ''
			)  AS codes
			FROM													filter
			GROUP BY filter.us_id 
	)
	*/
	
	-- Здесь список дополнительных ОКВЭД со строкой всех родителей - из XML (вариант 2):
	,sector_okved_code_additional_parent_list_xml_v2 AS
	(
		SELECT filter.us_id, q.codes
			FROM													filter
			CROSS APPLY
			(
				SELECT 
					REPLACE(
						STUFF
						(
							(
								SELECT 
									', ' + t.sector_okved_code_parent_list AS codes
									FROM										sector_okved_code_additional_parent_list t
									WHERE t.us_id = filter.us_id
									FOR XML PATH(''), TYPE
							).value('.', 'varchar(max)') 
						,1, 2, ''
						)
					,' ', ''
				)
				AS codes  
			) q
	)
	
	--SELECT * FROM sector_okved_code_additional_parent_list order by us_id		-- Здесь список дополнительных ОКВЭД со строкой всех родителей
	--SELECT * FROM sector_okved_code_additional_parent_list_xml_v1 x 			-- Здесь список дополнительных ОКВЭД со строкой всех родителей - из XML (вариант 1)
	SELECT * FROM sector_okved_code_additional_parent_list_xml_v2 x 			-- Здесь список дополнительных ОКВЭД со строкой всех родителей - из XML (вариант 2)
		WHERE x.codes IS NOT NULL
		--ORDER BY x.us_id