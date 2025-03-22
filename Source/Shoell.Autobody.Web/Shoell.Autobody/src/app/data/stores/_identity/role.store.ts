import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import Guid from "devextreme/core/guid";
import { DataFieldTypes, EntityEndpoint } from "src/app/classes";
import { Role, RoleODataStoreModel } from "src/app/data/models";
import { DataEventService } from "src/app/shared/services";
import { environment } from "src/environments/environment";
import { CoreStore } from "..";
import ODataStore from "devextreme/data/odata/store";

const url: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.Role}`;

@Injectable({
    providedIn: 'root'
})
export class RoleStore extends CoreStore<Role, Guid> {
    override url = url;
    override recycle: ODataStore<Role, Guid>;

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
            fieldTypes: RoleODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(Role, data[i]);
                }
            },
            onModified: () => {
                dataEventService.informRoleChanged();
            },
            onUpdating: (key: any, values: any) => {
                delete values.logs;
                delete values.roleGroups;
                delete values.userRoles;
            }
        };
        super(config);

        config.url = `${url}/Recycle`;
        this.recycle = new ODataStore(config);


        this.archive = this.archive.bind(this);
    }

    protected override informChanged(): void {
        this.dataEventService.informRoleChanged();
    }
}