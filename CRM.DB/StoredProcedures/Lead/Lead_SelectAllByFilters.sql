CREATE PROCEDURE [dbo].[Lead_SelectAllByFilters]
	@FirstName NVARCHAR (50),
	@LastName  NVARCHAR (50),
	@Patronymic NVARCHAR (50),
	@Role INT,
	@tblCities Cities readonly,
	@BirthDateFrom date,
	@BirthDateTo date,
	@SearchType INT
AS
BEGIN
	IF @SearchType = 1
	SET @FirstName = @FirstName + '%'
	SET @LastName = @LastName + '%'
	SET @Patronymic = @Patronymic + '%'

	IF @SearchType = 2
	SET @FirstName = '%' + @FirstName + '%'
	SET @LastName = '%' + @LastName + '%'
	SET @Patronymic = '%' + @Patronymic + '%'

	IF @SearchType = 3
	SET @FirstName = '%' + @FirstName
	SET @LastName = '%' + @LastName
	SET @Patronymic = '%' + @Patronymic

	SELECT 
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.Email,
		l.BirthDate,
		c.Id,
		c.Name,
		l.Role as Id,
		l.RegistrationDate
	FROM dbo.[Lead] l
	INNER JOIN City c on c.Id = l.CityId
	INNER JOIN @tblCities tc on tc.CityId = l.CityId
	WHERE (@FirstName IS NULL OR l.FirstName LIKE @FirstName) AND
	(@LastName IS NULL OR l.LastName LIKE @LastName) AND
	(@Patronymic IS NULL OR l.Patronymic LIKE @Patronymic) AND
	(@Role IS NULL OR l.Role LIKE @Role) AND
	(@BirthDateFrom IS NULL OR l.BirthDate >= @BirthDateFrom) AND
	(@BirthDateTo IS NULL OR l.BirthDate <= @BirthDateTo) AND
	(IsDeleted = 0)
END
