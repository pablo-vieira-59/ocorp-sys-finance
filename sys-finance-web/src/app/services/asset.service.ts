import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AssetDto, AssetHistoryDto } from './finance-models';

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  private apiUrl = environment.baseUrl + '/api/finance';

  constructor(private http: HttpClient) { }

  getAssets() { return this.http.get<AssetDto[]>(`${this.apiUrl}/assets`); }
  addAsset(asset: AssetDto) { return this.http.post<AssetDto>(`${this.apiUrl}/assets`, asset); }
  updateAsset(id: string, asset: AssetDto) { return this.http.put<AssetDto>(`${this.apiUrl}/assets/${id}`, asset); }
  deleteAsset(id: string) { return this.http.delete<any>(`${this.apiUrl}/assets/${id}`); }

  addAssetHistory(data: AssetHistoryDto) { return this.http.post<any>(`${this.apiUrl}/asset-history`, data); }
  deleteAssetHistory(id: string) { return this.http.delete<any>(`${this.apiUrl}/asset-history/${id}`); }
}