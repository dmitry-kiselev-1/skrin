
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

	-- Здесь выручка
	,revenue AS
	(
		SELECT 
			--value.issuer_id,
			--value.[year],
			--value.value
			filter.us_id,
			value.* 
			FROM
		(
			SELECT filter.gks_id, max(years.[year]) [year]
				FROM filter
				JOIN 
					gks.dbo.GKS_QIVInfo years
					ON years.issuer_id = filter.gks_id
				GROUP BY filter.gks_id
		) years
		JOIN gks.DBO.gks_QUART_INDIC_VALUES value
			ON value.indic_id = 50
			AND value.issuer_id = years.gks_id
			AND value.[year] = years.[year]
		JOIN filter
			ON filter.gks_id = years.gks_id
	)

	SELECT 
			--revenue.*
			--revenue.issuer_id,
			revenue.us_id,
			revenue.[year],
			revenue.value
	FROM revenue
