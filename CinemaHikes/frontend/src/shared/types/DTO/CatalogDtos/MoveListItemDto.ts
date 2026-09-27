import type { GenreDto } from "./GenreDto";

export  interface MovieListItem  {
    id:number;
    ruTitle:string;
    uaTitle?:string;
    ruInEngTitle:string;
    releaseYear:number;
    posterUrl:string;
    kpRating:number;
    Genres:GenreDto[];

}