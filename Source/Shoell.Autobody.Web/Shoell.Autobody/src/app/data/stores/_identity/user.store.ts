import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import Guid from "devextreme/core/guid";
import { Common, DataFieldTypes, EntityEndpoint } from "src/app/classes";
import { Role, User, UserODataStoreModel } from "src/app/data/models";
import { DataEventService } from "src/app/shared/services";
import { environment } from "src/environments/environment";
import { CoreStore } from "..";
import { catchError, lastValueFrom, of } from "rxjs";
import ODataStore from "devextreme/data/odata/store";

const url: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.User}`;
const meUrl: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.Me}`;

@Injectable({
    providedIn: 'root'
})
export class UserStore extends CoreStore<User, Guid> {
    override url = url;
    override recycle: ODataStore<User, Guid>;

    constructor(
        override dataEventService: DataEventService,
        override http: HttpClient
    ) {
        let config = {
            url: url,
            errorHandler: (error: any) => {
                console.error(error);
            },
            version: 4,
            key: 'id',
            keyType: DataFieldTypes.Guid,
            fieldTypes: UserODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(User, data[i]);
                }
            },
            onModified: () => {
                dataEventService.informUserChanged();
            },
            onUpdating: (key: any, values: any) => {
                delete values.logs;
                delete values.userGroups;
                delete values.roleGroups;
            }
        };
        super(config);

        config.url = `${url}/Recycle`;
        this.recycle = new ODataStore(config);

        this.archive = this.archive.bind(this);
    }

    async roles(options: any = {}): Promise<Role[]> {
        let response: any = await lastValueFrom(this.http
            .get(`${meUrl}/Roles()`, { params: options })
            .pipe(
                catchError(error => {
                    Common.toastError("An error occurred while fetching current user's roles.");
                    return of();
                })
            ));
        return response.value.map((entity: any) => plainToInstance(Role, entity));
    }

    async info(options: any = {}): Promise<User> {
        let response: any = await lastValueFrom(this.http
            .get(`${meUrl}/Info()`, { params: options })
            .pipe(
                catchError(error => {
                    Common.toastError("An error occurred while fetching current user's information.");
                    return of();
                })
            ));
        return plainToInstance(User, response);
    }

    override informChanged(): void {
        this.dataEventService.informUserChanged();
    }
}