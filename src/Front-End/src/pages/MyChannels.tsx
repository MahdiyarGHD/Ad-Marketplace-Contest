import { memo, useEffect } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Avatar from "../components/Avatar";
import { PlusIcon } from "lucide-react";
import "./MyChannels.scss";
import { useNavigate } from "react-router-dom";

function MyChannels() {
	const navigate = useNavigate();

	useEffect(() => {
		// mainButton.setText("Add Your Channel");
		// mainButton.show();
	}, []);

	return (
		<div className="MyChannels">
			<PageHeader>
				<PageHeaderTitle>My Channels</PageHeaderTitle>
			</PageHeader>

			<div className="ChatList">
				<div className="Item primary" onClick={() => navigate("/add-channel")}>
					<div className="icon">
						<PlusIcon />
					</div>
					<div className="title">Add Your Channel</div>
				</div>
				<div className="ChatItem">
					<Avatar id="1" title="Channel 1" photo="" />
					<div className="body">
						<div className="title">Channel 1</div>
						<div className="subtitle">Subtitle 1</div>
					</div>
					<div className="meta">Pending</div>
				</div>
			</div>

			<div className="NoChannel"></div>
		</div>
	);
}

export default memo(MyChannels);
