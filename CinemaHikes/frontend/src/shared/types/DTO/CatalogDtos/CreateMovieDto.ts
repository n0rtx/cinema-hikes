export interface CreateMovieDto {
    ruTitle:string;
    uaTitle:string;
    ruInEngTitle:string;
    description:string;
    director:string;
    releaseYear:number;
    posterUrl:string;
    kpRating:string;
    genreIds:number[];
}