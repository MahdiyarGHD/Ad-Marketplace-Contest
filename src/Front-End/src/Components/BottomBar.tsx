import { useLaunchParams } from "@tma.js/sdk-react";
import { memo } from "react";

function BottomBar({ }) {
    const launchParams = useLaunchParams()

    const firstName = launchParams.tgWebAppData?.user?.first_name;

    return <div className="BottomBar">
        <div className="Item">
            <div className="meta"></div>
            <div className="title">Home</div>
        </div>
        <div className="Item">
            <div className="meta">
                <div className="Avatar">
                    <div className="title">{firstName?.charAt(0).toUpperCase()}</div>
                </div>
            </div>
            <div className="title">{firstName}</div>
        </div>
    </div>
}

export default memo(BottomBar)