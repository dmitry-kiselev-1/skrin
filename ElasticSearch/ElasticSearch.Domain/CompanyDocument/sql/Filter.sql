
--DECLARE @BufferCount INT = 1000
--DECLARE @LastId INT = 0
--DECLARE @LastCompleted DATETIME = '1753-01-01T00:00:00.000'

IF OBJECT_ID(N'tempdb..##elastic_loader_companyid_buffer') IS NULL
	begin
		CREATE TABLE ##elastic_loader_companyid_buffer (us_id int UNIQUE CLUSTERED );
	end
ELSE
	begin
		TRUNCATE TABLE ##elastic_loader_companyid_buffer
	end

-- Список идентификаторов компаний для текущей операции:
INSERT ##elastic_loader_companyid_buffer
		SELECT 
			TOP (@BufferCount)
			s.ID
		FROM searchdb2.dbo.union_search s
		WHERE 
				s.ID > @LastId
			AND s.us_update_date > @LastCompleted
			AND s.uniq = 1 
			--AND COALESCE(s.inn, s.ogrn) IS NOT NULL
			--AND ( (LEN(RTRIM(LTRIM(s.inn))) > 0) OR (LEN(RTRIM(LTRIM(s.ogrn))) > 0) )
		ORDER BY s.ID;

--SELECT * FROM ##elastic_loader_companyid_buffer
--DROP TABLE ##elastic_loader_companyid_buffer
