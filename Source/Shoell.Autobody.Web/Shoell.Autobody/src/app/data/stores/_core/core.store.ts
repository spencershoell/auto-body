import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import ODataStore from "devextreme/data/odata/store";
import { lastValueFrom } from "rxjs";
import { DataEventService } from "src/app/shared/services";
import { Log, LogODataStoreModel } from "src/app/data/models";
import { DataFieldTypes } from "src/app/classes";


@Injectable({
    providedIn: 'root'
})
export abstract class CoreStore<TEntity, TKey> extends ODataStore<TEntity, TKey> {
    protected abstract dataEventService: DataEventService;
    protected abstract http: HttpClient

    protected abstract url: string;

    protected abstract recycle: ODataStore<TEntity, TKey>;

    constructor(options: any) {
        super(options);

        this.archive = this.archive.bind(this);
        this.informChanged = this.informChanged.bind(this);
    }

    async archive(key: TKey): Promise<void> {
        await lastValueFrom(this.http.post(`${this.url}(${key})/Archive`, {}));
        this.informChanged();
    }

    async restore(key: TKey): Promise<void> {
        await lastValueFrom(this.http.post(`${this.url}(${key})/Restore`, {}));
        this.informChanged();
    }

    async recover(key: TKey): Promise<void> {
        await lastValueFrom(this.http.post(`${this.url}(${key})/Recover`, {}));
        this.informChanged();
    }

    async purge(key: TKey): Promise<void> {
        await lastValueFrom(this.http.post(`${this.url}(${key})/Purge`, {}));
        this.informChanged();
    }

    getLogsStore(key: TKey): ODataStore {
        return new ODataStore({
            url: `${this.url}(${key})/Logs()`,
            errorHandler: (error: any) => {
                console.error(error);
            },
            version: 4,
            key: 'id',
            keyType: DataFieldTypes.Guid,
            fieldTypes: LogODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(Log, data[i]);
                }
            },
        });
    }

    protected abstract informChanged(): void;

}