export interface BaseApiResponse<T> {
    data: T;
    success: boolean;
    error: string;
}
