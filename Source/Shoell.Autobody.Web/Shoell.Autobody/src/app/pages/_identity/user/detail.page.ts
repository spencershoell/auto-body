import { NgIf } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterModule } from "@angular/router";
import { DxLoadIndicatorModule } from "devextreme-angular";
import Guid from "devextreme/core/guid";
import { Context, User } from "src/app/data";
import { UserDetailComponent } from "src/app/shared/components/_identity/user/detail.component";

@Component({
    selector: 'sh-user-detail-page',
    templateUrl: 'detail.page.html',
    styleUrls: ['detail.page.scss'],
    imports: [NgIf, RouterModule, DxLoadIndicatorModule, UserDetailComponent]
})
export class UserDetailPageComponent implements OnInit {

    id!: Guid;
    model?: User;

    constructor(private route: ActivatedRoute, private context: Context) { }

    async ngOnInit(): Promise<void> {
        this.id = this.route.snapshot.paramMap.get("id") as Guid;
        await this.refresh();
    }

    async refresh(): Promise<void> {
        this.model = (await this.context.users.load({ filter: ['id', this.id] }))[0];
    }
}