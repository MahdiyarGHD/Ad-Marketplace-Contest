import { memo, useEffect } from "react";
import RLottie from "../components/RLottie";
import { backButton, mainButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router";
import "./SetChannelData.scss";

function SetChannelData() {
	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	useEffect(() => {
		mainButton.setText("Back to Channels");
		mainButton.onClick(onBackButton);
		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			mainButton.offClick(onBackButton);

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	const renderSuccess = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="congrats" autoplay width={120} height={120} />
				</div>
				<h2 className="Title">Your Channel Successfully Added</h2>
				<div className="Instructions"></div>
			</div>
		);
	};

	return <div className="SetChannelData">{renderSuccess()}</div>;
}

export default memo(SetChannelData);
